using System;
using System.Threading;
using System.Threading.Tasks;
using SkriptInsight.Core.Files.Processes;
using SkriptInsight.Core.Files.Processes.Impl;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace SkriptInsight.Core.Files
{
    public class SkriptFile
    {
        public SkriptFile(Uri url)
        {
            Url = url;
            ParseContext = new FileParseContext(this);
        }

        public Uri Url { get; set; }

        public string[] RawContents { get; set; } = { };

        public FileParseContext ParseContext { get; }

        public void RunProcess(FileProcess process, int startLine = -1, int endLine = -1)
        {
            startLine = Math.Max(0, startLine);
            endLine = Math.Min(RawContents.Length, endLine);

            Parallel.For(startLine, endLine + 1,
                new ParallelOptions {MaxDegreeOfParallelism = Environment.ProcessorCount},
                (i, i2) =>
                {
                    var threadCount = 0;
                    Interlocked.Increment(ref threadCount);

                    process.DoWork(this, i, RawContents[i]);

                    Interlocked.Decrement(ref threadCount);
                }
            );
        }

        public void PreProcess()
        {
            
        }
    }
}