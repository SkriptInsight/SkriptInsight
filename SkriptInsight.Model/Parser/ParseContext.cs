using System;
using System.Collections.Generic;

namespace SkriptInsight.Model.Parser
{
    public class ParseContext
    {
        private ParseContext()
        {
        }

        public static ParseContext FromCode(string code)
        {
            return new ParseContext
            {
                Text = code
            };
        }

        public string Text { get; set; }

        public int CurrentPosition { get; set; }

        public string PeekNext(int count)
        {
            return Text.Substring(CurrentPosition, count);
        }

        public string ReadNext(int count)
        {
            var result = PeekNext(count);
            CurrentPosition += count;
            return result;
        }

        public string ReadUntilPosition(int pos)
        {
            return ReadNext(pos - CurrentPosition);
        }

        public string ReadUntilEnd()
        {
            var result = Text.Substring(CurrentPosition);
            CurrentPosition = Text.Length;
            return result;
        }

        public ParseContext Clone()
        {
            return this.JsonClone();
        }

        public int FindNextBracket(char opening, char closing)
        {
            var openedBracketStack = new Stack<int>();
            var finalChar = -1;
            for (var i = CurrentPosition; i < Text.Length; i++)
            {
                var ch = Text[i];

                if (ch.Equals(closing))
                {
                    if (openedBracketStack.Count == 0)
                    {
                        finalChar = i;
                        break;
                    }
                    openedBracketStack.Pop();
                } else if (ch.Equals(opening))
                {
                    openedBracketStack.Push(i);
                }
            }

            return finalChar;
        }
    }
}