using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MoreLinq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Files.Nodes;
using SkriptInsight.Core.Files.Nodes.Impl;
using SkriptInsight.Core.Files.Processes;
using SkriptInsight.Core.Files.Processes.Impl;
using SkriptInsight.Core.Inspections.Problems;
using SkriptInsight.Core.Managers;
using SkriptInsight.Core.Parser;
using static SkriptInsight.Core.Files.Processes.Impl.ProcCreateOrUpdateNodeChildren;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace SkriptInsight.Core.Files
{
    public class SkriptFile
    {
        private static DocumentSelector _selector;
        private FileProcess _parseProcess;
        private FileProcess _inspectProcess;

        public SkriptFile(Uri url)
        {
            Url = url;
            ParseContext = new FileParseContext(this);
        }

        public static DocumentSelector Selector => _selector ??= DocumentSelector.ForLanguage("skriptlang");

        public Uri Url { get; set; }

        public List<string> RawContents { get; set; } = new List<string>();

        public ConcurrentNodeDictionary Nodes { get; } =
            new ConcurrentNodeDictionary();

        public FileParseContext ParseContext { get; }

        public FileProcess ParseProcess
        {
            get => _parseProcess ??= ProvideParseProcess();
            set => _parseProcess = value;
        }
        
        public FileProcess InspectProcess
        {
            get => _inspectProcess ??= ProvideInspectProcess();
            set => _inspectProcess = value;
        }
        
        [CanBeNull] public List<Range> VisibleRanges { get; set; }

        public void HandleChange(TextDocumentContentChangeEvent edit)
        {
            if (edit.Range == null)
            {
                RawContents.AddRange(edit.Text.SplitOnNewLines());
                return;
            }

            var startLine = (int) edit.Range.Start.Line;
            var endLine = (int) edit.Range.End.Line;

            var lineCount = endLine - startLine + 1;
            var oldStrings = RawContents.GetRange(startLine, lineCount);
            RawContents.RemoveRange(startLine, lineCount);

            var customRange = edit.Range.ModifyPositions(pos => { pos.Line -= edit.Range.Start.Line; });
            var startPos = customRange.Start.ResolveFor(oldStrings);
            var endPos = customRange.End.ResolveFor(oldStrings);
            var sb = new StringBuilder(string.Join(Environment.NewLine, oldStrings));

            sb.Remove(startPos, endPos - startPos);
            sb.Insert(startPos, edit.Text);

            var finalStrings = sb.ToString().SplitOnNewLines();
            RawContents.InsertRange(startLine, finalStrings);

            if (finalStrings.Count > lineCount)
            {
                var linesNumber = finalStrings.Count - lineCount;
                for (var i = startLine; i <= startLine + linesNumber; i++) Nodes[i] = null;

                var shiftRangeStart = startLine + 1;
                Nodes.ShiftRangeRight(shiftRangeStart, Nodes.Count - shiftRangeStart, linesNumber,
                    n => n.ShiftLineNumber(linesNumber));
            }
            else if (finalStrings.Count < lineCount)
            {
                var amount = Math.Abs(finalStrings.Count - lineCount);
                var removedLineNumber = startLine + lineCount + (finalStrings.Count - lineCount);

                for (var i = removedLineNumber; i < removedLineNumber + amount; i++) Nodes[i] = null;

                Nodes.ShiftRangeLeft(removedLineNumber + amount, Nodes.Count - removedLineNumber, amount,
                    n => n.ShiftLineNumber(-amount));
            }

            PrepareNodes(startLine, startLine + finalStrings.Count + (finalStrings.Count - lineCount));

            Nodes.SkipWhile(kv => kv.Value != null).ToList().ForEach(c => Nodes.Remove(c.Key, out _));
        }

        public void RunProcess(FileProcess process, int startLine = -1, int endLine = -1)
        {
            if (process == null) return;

            startLine = Math.Max(0, startLine);
            endLine = endLine < 0 ? RawContents.Count : endLine;
            var maxDegreeOfParallelism = Environment.ProcessorCount;
            var contexts = new ConcurrentQueue<FileParseContext>(Enumerable
                .Range(0, (int) Math.Ceiling(maxDegreeOfParallelism * 1.20))
                .Select(_ => new FileParseContext(this) {MoveToNextLine = false}).ToList());

            var sw = Stopwatch.StartNew();

            WorkspaceManager.CurrentHost.LogInfo(
                $"Starting {process.GetType().Name} on {endLine - startLine} line(s).");
            Parallel.For(startLine, endLine + 1,
                new ParallelOptions {MaxDegreeOfParallelism = maxDegreeOfParallelism},
                line =>
                {
                    if (RawContents.ElementAtOrDefault(line) != null)
                    {
                        var rawContent = RawContents.ElementAt(line);
                        contexts.TryDequeue(out var context);
                        context.CurrentMatchStack.Clear();
                        context.TemporaryRangeStack.Clear();
                        context.Matches.Clear();
                        context.IndentationChars = rawContent.TakeWhile(char.IsWhiteSpace).Count();

                        context.CurrentLine = line;

                        process.DoWork(this, line, rawContent, context);

                        contexts.Enqueue(context);
                    }
                });

            if (process == ParseProcess && GetType() == typeof(SkriptFile))
            {
                var diags = new List<Diagnostic>();
                Nodes.GetRange(startLine, endLine + 1).ForEach(node =>
                {
                    var matches = node.MatchedSyntax?.Result.Matches;
                    if (matches == null) return;

                    diags.AddRange(matches.OfType<ExpressionParseMatch>().Explode()
                        .Select(c => (Expression: c,
                            Matches: c.MatchAnnotations.Where(match => match.ShouldBeDiagnostic)))
                        .Select(c => c.Matches.Select(match => match.ToDiagnostic(c.Expression)))
                        .SelectMany(diagnostics => diagnostics));


                    if (!(node is CommentLineNode) && node.MatchedSyntax == null)
                        diags.Add(
                            new Diagnostic
                            {
                                Code = "1",
                                Message = "This node doesn't match any syntax!",
                                Range = node.ContentRange,
                                Severity = DiagnosticSeverity.Warning,
                                Source = "SkriptInsight"
                            });
                });
                WorkspaceManager.CurrentHost.PublishDiagnostics(Url, diags);
            }

            WorkspaceManager.CurrentHost.LogInfo(
                $"Took {sw.ElapsedMilliseconds}ms to run {process.GetType().Name} on {endLine - startLine} line(s) [{startLine}->{endLine}].");
        }

        protected virtual FileProcess ProvideParseProcess()
        {
            return new ProcTryParseEffects();
        }

        protected virtual FileProcess ProvideInspectProcess()
        {
            return new ProcInspectCode(new ProblemHolder());
        }


        public void PrepareNodes(int startLine = -1, int endLine = -1)
        {
            startLine = Math.Max(0, startLine);
            endLine = endLine < 0 ? RawContents.Count : endLine;
            RunProcess(new ProcCreateOrUpdateNodes(), startLine, endLine);
            ProcessNodeIndentation(startLine);
            if (!((WorkspaceManager.CurrentHost?.SupportsExtendedCapabilities ?? false) &&
                  (WorkspaceManager.CurrentHost?.ExtendedCapabilities?.SupportsViewportReporting ?? false)))
            {
                RunProcess(ParseProcess);
                RunProcess(InspectProcess);
            }
        }

        internal void ProcessNodeIndentation(in int startLine)
        {
            var nodes = Nodes.GetRange(startLine, Nodes.Count + 1);
            var fileNodes = nodes as AbstractFileNode[] ?? nodes.ToArray();

            var firstNode = fileNodes.FastElementAtOrDefault(0);
            var firstIndent = firstNode != null ? GetIndentCount(firstNode) : 0;

            var indentLevels = new[] {0}
                .Concat(
                    fileNodes.Where(n => n.Indentations.Length < 2)
                        .TakeWhile(n => GetIndentCount(n) == firstIndent || IsChildrenAccordingToIndent(n, firstIndent))
                        .SelectMany(c => c.Indentations)
                        .GroupBy(i => i.Count)
                        .Select(c => c.Key)
                ).ToList();

            foreach (var level in indentLevels) RunProcess(new ProcCreateOrUpdateNodeChildren(level));
        }

        public void NotifyVisibleNodesRangeChanged()
        {
            if (VisibleRanges == null) return;

            foreach (var range in VisibleRanges)
            {
                WorkspaceManager.CurrentHost.LogInfo(
                    $"Selectively Parsing nodes from {range.Start.Line} to {range.End.Line}.");
                var (start, end) = Nodes.ExpandRange((int) range.Start.Line, (int) range.End.Line);
                RunProcess(ParseProcess, start, end);
                RunProcess(InspectProcess, start, end);
            }
            
        }
    }
}