using System;
using System.Collections.Generic;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace SkriptInsight.Core.Extensions
{
    public static class RangeExtensions
    {
        public static Range Clone(this Range range)
        {
            return From(range.Start.Line, range.Start.Character, range.End.Line, range.End.Character);
        }
        
        public static Range From(long startLine, long startCharacter, long endLine, long endCharacter)
        {
            return new Range(new Position(startLine, startCharacter), new Position(endLine, endCharacter));
        }
        public static Range From(long line, long startCharacter, long endCharacter)
        {
            return From(line, startCharacter, line, endCharacter);
        }
        
        public static Range ModifyPositions(this Range range, Action<Position> action)
        {
            var startPos = new Position(range.Start.Line, range.Start.Character);
            var endPos = new Position(range.End.Line, range.End.Character);
            action(startPos);
            action(endPos);
            return new Range(startPos, endPos);
        }

        public static Range OffsetStart(this Range range, int character = 0, int line = 0)
        {
            return new Range(new Position(range.Start.Line + line, range.Start.Character + character), range.End);
        }

        public static Range OffsetEnd(this Range range, int character = 0, int line = 0)
        {
            return new Range(range.Start, new Position(range.End.Line + line, range.End.Character + character));
        }

        public static Range Offset(this Range range, int character = 0, int line = 0)
        {
            return range.OffsetStart(character, line).OffsetEnd(character, line);
        }

        public static void ShiftLineNumber(this Position pos, int amount)
        {
            pos.Line += amount;
        }
        

        public static void ShiftLineNumber(this Range range, int amount)
        {
            range.Start.ShiftLineNumber(amount);
            range.End.ShiftLineNumber(amount);
        }
        
        public static int ResolveFor(this Position pos, List<string> str)
        {
            var targetLine = pos.Line;
            if (str.Count < targetLine)
                targetLine = str.Count - 1;
            
            var lineBreakSize = Environment.NewLine.Length;
            var charsUntilLine = 0;
            for (var index = 0; index < str.Count; index++)
            {
                var line = str[index];
                if (index < targetLine)
                {
                    charsUntilLine += line.Length + lineBreakSize;
                }
                else
                {
                    charsUntilLine += (int) pos.Character;
                    break;
                }
            }

            return charsUntilLine;
        }
    }
}