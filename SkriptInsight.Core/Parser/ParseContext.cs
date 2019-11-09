using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Parser.Patterns;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace SkriptInsight.Core.Parser
{
    [JsonObject]
    public class ParseContext : IEnumerable<char>
    {
        private int _indentationChars;

        public ParseContext()
        {
        }

        public virtual string Text { get; set; } = "";

        public virtual int CurrentLine { get; set; }

        public int CurrentPosition { get; set; }

        public bool HasFinishedLine => CurrentPosition >= Text.Length;

        public bool HasReachedEndOfEnumerator => CurrentPosition > Text.Length;

        public ContextualElement<AbstractSkriptPatternElement> ElementContext { get; set; }

        public int IndentationChars
        {
            get => _indentationChars;
            set
            {
                _indentationChars = value;

                if (CurrentPosition < value)
                    CurrentPosition = value;
            }
        }

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

        public virtual string PeekNext(int count)
        {
            return CurrentPosition + count > Text.Length ? "" : Text.Substring(CurrentPosition, count);
        }

        public string PeekPrevious(int count)
        {
            return CurrentPosition - count < 0 || CurrentPosition > Text.Length
                ? ""
                : Text.Substring(Math.Clamp(CurrentPosition - count, 0, Text.Length), count);
        }

        public virtual string ReadNext(int count)
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
            var result = PeekUntilEnd();
            CurrentPosition = Text.Length;
            return result;
        }

        public string PeekUntilEnd()
        {
            var result = Text.Substring(CurrentPosition);
            return result;
        }

        [Pure]
        public virtual ParseContext Clone(bool includeMatches = true)
        {
            return new ParseContext
            {
                Text = Text,
                Matches = includeMatches ? Matches.ToList() : new List<ParseMatch>(),
                CurrentLine = CurrentLine,
                CurrentPosition = CurrentPosition,
                ElementContext = ElementContext,
                CurrentMatchStack = new Stack<int>(CurrentMatchStack.Reverse()),
                TemporaryRangeStack = new Stack<int>(TemporaryRangeStack.Reverse())
            };
        }


        public int FindNextCharNotInsideNormalBracket(char ch, bool returnLengthOnFail = false,
            bool returnOnUnopenedBrackets = true)
        {
            var openCloseStack = new KeyedStack<char, int>();
            var posStack = new Stack<int>();
            (char Opening, char Closing, bool DoubleEscape)[] brackets =
            {
                ('(', ')', false),
                ('[', ']', false),
                ('"', '"', true)
            };


            for (var i = CurrentPosition; i < Text.Length; i++)
            {
                var forceBreak = false;
                var firstPos = i;
                foreach (var (opening, closing, doubleEscape) in brackets)
                {
                    var currentChar = Text.ElementAtOrDefault(i);
                    if (!doubleEscape)
                    {
                        if (currentChar == opening)
                            posStack.Push(i);
                        else if (currentChar == closing)
                        {
                            if (posStack.Count > 0)
                            {
                                posStack.Pop();
                            }
                            else if (returnOnUnopenedBrackets)
                            {
                                // Return now because the callee requested
                                forceBreak = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        var isEqualChar = opening == closing;
                        var hasBeenDoubleEscaped = Text.ElementAtOrDefault(i + 1) == currentChar;
                        if (hasBeenDoubleEscaped)
                        {
                            i++;
                            break;
                        }

                        if (isEqualChar)
                        {
                            if (currentChar != opening) continue;

                            if (openCloseStack[opening].Count % 2 != 0)
                            {
                                openCloseStack[opening].Pop();
                                posStack.Pop();
                            }
                            else
                            {
                                openCloseStack[opening].Push(i);
                                posStack.Push(i);
                            }
                        }
                        else
                        {
                            if (currentChar == opening)
                                posStack.Push(i);
                            else if (currentChar == closing) posStack.Pop();
                        }
                    }
                }

                if (!forceBreak && i != firstPos)
                    continue;

                if (posStack.Count == 0 && (forceBreak || Text.ElementAtOrDefault(i) == ch))
                    return i;
            }

            return returnLengthOnFail ? Text.Length : -1;
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
                    //TODO: Check if match exclusions are matched by pair
                    if (matchExclusions?.Any(c =>
                            ch == c.Item1 || c.Item1 == c.Item2 && openedBracketStack.Count % 2 == 0) ?? false)
                        exclusionStack.Push(i);
                    else if (matchExclusions?.Any(c =>
                                 ch == c.Item2 || c.Item1 == c.Item2 && openedBracketStack.Count % 2 != 0 &&
                                 exclusionStack.Count > 0) ??
                             false) exclusionStack.Pop();


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

        [JsonIgnore] public List<ParseMatch> Matches { get; set; } = new List<ParseMatch>();

        [System.Text.Json.Serialization.JsonIgnore]
        public Stack<int> CurrentMatchStack { get; set; } = new Stack<int>();

        [System.Text.Json.Serialization.JsonIgnore]
        public Stack<int> TemporaryRangeStack { get; set; } = new Stack<int>();

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

        public ParseMatch EndMatch(bool save = false)
        {
            var startingPos = CurrentMatchStack.Pop();
            var endingPos = Math.Min(CurrentPosition, Text.Length);
            var result = new ParseMatch
            {
                Context = this,
                Range = new Range(new Position(CurrentLine, startingPos), new Position(CurrentLine, endingPos)),
                RawContent = Text.SafeSubstring(startingPos, endingPos - startingPos)
            };
            if (save)
                Matches.Add(result);
            return result;
        }

        #endregion

        public static implicit operator ParseContext(string code) => FromCode(code);
    }
}