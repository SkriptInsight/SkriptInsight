using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;
using SkriptInsight.Core.Parser;

namespace SkriptInsight.Core.Files
{
    public class FileParseContext : ParseContext
    {
        private int _currentLine;

        public bool MoveToNextLine { get; set; } = true;

        public FileParseContext(SkriptFile file)
        {
            File = file;
        }

        [JsonIgnore] public SkriptFile File { get; set; }

        public override string Text
        {
            get => File.Nodes?.ElementAtOrDefault(CurrentLine)?.NodeContent ??
                   File.RawContents.ElementAtOrDefault(CurrentLine) ?? string.Empty;
            set => throw new NotSupportedException();
        }

        public bool HasReachedEndOfFile => CurrentLine > File.RawContents.Count ||
                                           CurrentLine == File.RawContents.Count && CurrentPosition >= Text.Length ||
                                           File.RawContents.All(string.IsNullOrEmpty);

        public override ParseContext Clone()
        {
            return new FileParseContext(File)
            {
                Matches = Matches.ToList(),
                CurrentLine = CurrentLine,
                MoveToNextLine = MoveToNextLine,
                _currentLine = _currentLine,
                CurrentPosition = CurrentPosition,
                ElementContext = ElementContext,
                CurrentMatchStack = new Stack<int>(CurrentMatchStack.Reverse()),
                TemporaryRangeStack = new Stack<int>(TemporaryRangeStack.Reverse())

            };
        }

        public override string ReadNext(int count)
        {
            var next = base.ReadNext(count);
            if (HasFinishedLine && MoveToNextLine)
            {
                //Line has been finished, so move to the next line.
                CurrentLine += 1;
                Debug.WriteLine($"Moved to next line whilst reading {count} chars.");
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