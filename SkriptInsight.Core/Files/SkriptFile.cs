using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MoreLinq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Files.Nodes;
using SkriptInsight.Core.Files.Processes;
using SkriptInsight.Core.Files.Processes.Impl;

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
            var contexts = Enumerable.Range(0, maxDegreeOfParallelism + 1).Select(_ => new FileParseContext(this))
                .ToArray();

            var processCount = -1;

            PrepareFileNodesSize();

            Parallel.For(startLine, endLine + 1,
                new ParallelOptions {MaxDegreeOfParallelism = maxDegreeOfParallelism},
                line =>
                {
                    Interlocked.Increment(ref processCount);
                   
                    if (RawContents.ElementAtOrDefault(line) != null)
                    {
                        var context = contexts[processCount];
                        context.CurrentLine = line;

                        process.DoWork(this, line, RawContents.ElementAt(line), context);
                    }

                    Interlocked.Decrement(ref processCount);
                });
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
        }
    }
}