using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MoreLinq;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Files.Nodes;
using SkriptInsight.Core.Files.Processes;
using SkriptInsight.Core.Files.Processes.Impl;
using SkriptInsight.Core.Managers;
using SkriptInsight.Core.Parser;
using SkriptInsight.Core.Parser.Expressions.Variables;

namespace SkriptInsight.Core.Files
{
    public class SkriptFile
    {
        private static DocumentSelector _selector;

        public static DocumentSelector Selector => _selector ??= DocumentSelector.ForLanguage("skriptlang");

        public SkriptFile(Uri url)
        {
            Url = url;
            ParseContext = new FileParseContext(this);
        }

        public Uri Url { get; set; }

        public List<string> RawContents { get; set; } = new List<string>();

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
                for (var i = startLine; i <= startLine + linesNumber; i++)
                {
                    Nodes[i] = null;
                }

                var shiftRangeStart = startLine + 1;
                Nodes.ShiftRangeRight(shiftRangeStart, Nodes.Count - shiftRangeStart, linesNumber,
                    n => n.ShiftLineNumber(linesNumber));
            }
            else if (finalStrings.Count < lineCount)
            {
                var amount = Math.Abs(finalStrings.Count - lineCount);
                var removedLineNumber = startLine + lineCount + (finalStrings.Count - lineCount);

                for (var i = removedLineNumber; i < removedLineNumber + amount; i++)
                {
                    Nodes[i] = null;
                }
                
                Nodes.ShiftRangeLeft(removedLineNumber + amount, Nodes.Count - removedLineNumber, amount,
                    n => n.ShiftLineNumber(-amount));
            }
            
            PrepareNodes(startLine, startLine + finalStrings.Count + (finalStrings.Count - lineCount));
            
            Nodes.SkipWhile(kv => kv.Value != null).ToList().ForEach(c => Nodes.Remove(c.Key, out _));
        }

        public NodesConcurrentDictionary Nodes { get; internal set; } =
            new NodesConcurrentDictionary();

        public FileParseContext ParseContext { get; }

        public void RunProcess(FileProcess process, int startLine = -1, int endLine = -1)
        {
            startLine = Math.Max(0, startLine);
            endLine = endLine < 0 ? RawContents.Count : endLine;
            var maxDegreeOfParallelism = Environment.ProcessorCount;
            var contexts = new ConcurrentQueue<FileParseContext>(Enumerable
                .Range(0, (int) Math.Ceiling(maxDegreeOfParallelism * 1.20))
                .Select(_ => new FileParseContext(this) {MoveToNextLine = false}).ToList());

            PrepareFileNodesSize();

            var sw = Stopwatch.StartNew();

            WorkspaceManager.Instance.Current.Server.Window.LogInfo(
                $"Starting {process.GetType().Name} on {endLine - startLine + 1} lines.");
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
                        context.Matches = new List<ParseMatch>();
                        context.IndentationChars = rawContent.TakeWhile(char.IsWhiteSpace).Count();

                        context.CurrentLine = line;

                        process.DoWork(this, line, rawContent, context);

                        contexts.Enqueue(context);
                    }
                });
            
            
            /*var diags = new List<Diagnostic>();

            Nodes.Select(c => c.Value).ForEach(node =>
            {
                if (node?.MatchedSyntax != null)
                {
                    var result = node.MatchedSyntax.Result;
                
                    diags.AddRange(result.Matches.OfType<ExpressionParseMatch>()
                        .SelectMany(r => r.Expression.GetValues<string>())
                        .Select(resultMatch => new Diagnostic
                        {
                            Code = "1",
                            Message = "Found a String",
                            Range = resultMatch.Range,
                            Severity = DiagnosticSeverity.Warning,
                            Source = "SkriptInsight"
                        }));
                
                    diags.AddRange(result.Matches.OfType<ExpressionParseMatch>()
                        .SelectMany(r => r.Expression.GetValues<SkriptVariable>())
                        .Select(resultMatch => new Diagnostic
                        {
                            Code = "2",
                            Message = "Found a variable. REEEEEE",
                            Range = resultMatch.Range,
                            Severity = DiagnosticSeverity.Error,
                            Source = "SkriptInsight"
                        }));
                }

            });
            
            WorkspaceManager.Instance.Current.Server.Document.PublishDiagnostics(new PublishDiagnosticsParams
            {
                Uri = Url,
                Diagnostics = diags
            });*/
            
            WorkspaceManager.Instance.Current.Server.Window.LogInfo(
                $"Took {sw.ElapsedMilliseconds}ms to run {process.GetType().Name} on {endLine - startLine + 1} lines [{startLine}->{endLine}].");
            
            if (process is ProcTryParseEffects)
            {
                MinecraftColoringManager.Instance.File_OnParseComplete(this, new EventArgs());
            }
            WorkspaceManager.Instance.Current.Server.Window.LogInfo($"Took {sw.ElapsedMilliseconds}ms to run {process.GetType().Name} on {endLine - startLine + 1} lines.");
        }

        private void PrepareFileNodesSize()
        {
//            var fileNodes = Nodes;
//            if (fileNodes.Count >= RawContents.Count) return;
//            Array.Resize(ref fileNodes, RawContents.Count);
//            Nodes = fileNodes;
        }

        public void PrepareNodes(int startLine = -1, int endLine = -1)
        {
            startLine = Math.Max(0, startLine);
            endLine = endLine < 0 ? RawContents.Count : endLine;
            RunProcess(new ProcCreateOrUpdateNodes(), startLine, endLine);
            RunProcess(new ProcTryParseEffects(), startLine, endLine);
        }
    }
}