using System;
using System.Linq;
using SkriptInsight.Core.Parser;

namespace SkriptInsight.Core.Files
{
    public class FileParseContext : ParseContext
    {
        private int _currentLine;

        public FileParseContext(SkriptFile file)
        {
            File = file;
        }

        public SkriptFile File { get; set; }

        public override string Text
        {
            get => File.RawContents.ElementAtOrDefault(CurrentLine) ?? string.Empty;
            set => throw new NotSupportedException();
        }

        public bool HasReachedEndOfFile => CurrentLine > File.RawContents.Length ||
                                           CurrentLine == File.RawContents.Length && CurrentPosition >= Text.Length ||
                                           File.RawContents.All(string.IsNullOrEmpty);

        public override string ReadNext(int count)
        {
            var next = base.ReadNext(count);
            if (HasFinishedLine)
            {
                //Line has been finished, so move to the next line.
                CurrentLine += 1;
            }

            return next;
        }

        public override int CurrentLine
        {
            get => _currentLine;
            set
            {
                _currentLine = value;
                //Reset position when changing lines
                CurrentPosition = 0;
            }
        }
    }
}