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
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Files.Nodes;
using SkriptInsight.Core.Files.Processes;
using SkriptInsight.Core.Files.Processes.Impl;
using SkriptInsight.Core.Managers;

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

            PrepareNodes(startLine, endLine);
        }

        public AbstractFileNode[] Nodes { get; internal set; } = { };

        public FileParseContext ParseContext { get; }

        public void RunProcess(FileProcess process, int startLine = -1, int endLine = -1)
        {
            startLine = Math.Max(0, startLine);
            endLine = endLine < 0 ? RawContents.Count : endLine;
            var maxDegreeOfParallelism = Environment.ProcessorCount;
            var contexts = new ConcurrentQueue<FileParseContext>(Enumerable.Range(0, (int) Math.Ceiling(maxDegreeOfParallelism * 1.20))
                .Select(_ => new FileParseContext(this) {MoveToNextLine = false}).ToList());

            PrepareFileNodesSize();

            var sw = Stopwatch.StartNew();
            
            WorkspaceManager.Instance.Current.Server.Window.LogInfo($"Starting {process.GetType().Name} on {endLine - startLine + 1} lines.");
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
            
            if (process is ProcTryParseEffects)
            {
                MinecraftColoringManager.Instance.File_OnParseComplete(this, new EventArgs());
            }
            WorkspaceManager.Instance.Current.Server.Window.LogInfo($"Took {sw.ElapsedMilliseconds}ms to run {process.GetType().Name} on {endLine - startLine + 1} lines.");
        }

        private void PrepareFileNodesSize()
        {
            var fileNodes = Nodes;
            if (fileNodes.Length >= RawContents.Count) return;
            Array.Resize(ref fileNodes, RawContents.Count);
            Nodes = fileNodes;
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