using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace SkriptInsight.Model.Parser
{
    [JsonObject]
    public class ParseContext : IEnumerable<char>
    {
        private ParseContext()
        {
        }

        public string Text { get; set; }

        public int CurrentLine { get; set; }

        public int CurrentPosition { get; set; }

        public bool HasFinishedLine => CurrentPosition >= Text.Length;
        
        public bool HasReachedEnd => CurrentPosition > Text.Length;

        public IEnumerator<char> GetEnumerator()
        {
            return new ParseContextEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static ParseContext FromCode(string code)
        {
            return new ParseContext
            {
                Text = code
            };
        }

        public string PeekNext(int count)
        {
            return CurrentPosition + count > Text.Length ? "" : Text.Substring(CurrentPosition, count);
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

        public int FindNextBracket(char bracket, (char, char)[] matchExclusions = null)
        {
            return FindNextBracket(bracket, bracket, true, false, matchExclusions);
        }

        public int FindNextBracket(char bracket, bool escapeByDouble = false, (char, char)[] matchExclusions = null)
        {
            return FindNextBracket(bracket, bracket, true, escapeByDouble, matchExclusions);
        }

        public int FindNextBracket(char opening, char closing, bool matchByPair = false, bool escapeByDouble = false,
            (char, char)[] matchExclusions = null)
        {
            var exclusionStack = new Stack<int>();
            var openedBracketStack = new Stack<int>();
            var finalChar = -1;

            if (matchByPair)
                openedBracketStack.Push(CurrentPosition);

            for (var i = CurrentPosition; i < Text.Length; i++)
            {
                var ch = Text[i];

                if (matchByPair)
                {
                    if (matchExclusions?.Any(c => ch == c.Item1) ?? false)
                        exclusionStack.Push(i);
                    else if (matchExclusions?.Any(c => ch == c.Item2) ?? false) exclusionStack.Pop();


                    if (ch.Equals(opening))
                    {
                        if (escapeByDouble && Text.ElementAtOrDefault(i + 1) == ch)
                        {
                            // Found a double escape. Ignore and skip
                            i++;
                            continue;
                        }

                        if (exclusionStack.Count == 0)
                        {
                            if (openedBracketStack.Count % 2 != 0)
                                openedBracketStack.Pop();
                            else
                                openedBracketStack.Push(i);
                        }

                        if (openedBracketStack.Count == 0)
                        {
                            finalChar = i;
                            break;
                        }
                    }

                    continue;
                }

                if (ch.Equals(closing))
                {
                    if (openedBracketStack.Count == 0)
                    {
                        finalChar = i;
                        break;
                    }

                    openedBracketStack.Pop();
                }
                else if (ch.Equals(opening))
                {
                    openedBracketStack.Push(i);
                }
            }

            return finalChar;
        }

        #region Match

        [JsonIgnore]
        public List<ParseMatch> Matches { get; } = new List<ParseMatch>();

        [System.Text.Json.Serialization.JsonIgnore] public Stack<int> CurrentMatchStack { get; } = new Stack<int>();
        [System.Text.Json.Serialization.JsonIgnore] public Stack<int> TemporaryRangeStack { get; } = new Stack<int>();

        public void StartRangeMeasure(string description = "")
        {
            TemporaryRangeStack.Push(CurrentPosition);
        }

        public Range EndRangeMeasure(string description = "")
        {
            var startingPos = TemporaryRangeStack.Pop();
            var endingPos = CurrentPosition;
            return new Range(new Position(CurrentLine, startingPos), new Position(CurrentLine, endingPos));
        }

        public void UndoRangeMeasure()
        {
            TemporaryRangeStack.TryPop(out _);
        }

        public void StartMatch()
        {
            CurrentMatchStack.Push(CurrentPosition);
        }

        public void UndoMatch()
        {
            CurrentMatchStack.TryPop(out _);
        }

        public ParseMatch EndMatch(bool save = true)
        {
            var startingPos = CurrentMatchStack.Pop();
            var endingPos = Math.Min(CurrentPosition, Text.Length);
            var result = new ParseMatch
            {
                Context = this,
                Range = new Range(new Position(CurrentLine, startingPos), new Position(CurrentLine, endingPos)),
                Content = Text.Substring(startingPos, endingPos - startingPos)
            };
            if (save)
                Matches.Add(result);
            return result;
        }

        #endregion

        public static implicit operator ParseContext(string code) => FromCode(code);
    }
}