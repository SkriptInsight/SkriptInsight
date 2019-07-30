using System;

namespace SkriptInsight.Model.Parser.Patterns
{
    [AttributeUsage(AttributeTargets.Class)]
    sealed class GroupPatternElementInfoAttribute : Attribute
    {
        public char OpeningBracket { get; }

        public char ClosingBracket { get; }
        
        public bool MatchBracketByPair { get; }

        public GroupPatternElementInfoAttribute(char openingBracket, char closingBracket)
        {
            OpeningBracket = openingBracket;
            ClosingBracket = closingBracket;
            MatchBracketByPair = false;
        }
        
        public GroupPatternElementInfoAttribute(char bracket)
        {
            OpeningBracket = bracket;
            ClosingBracket = bracket;
            MatchBracketByPair = true;
        }

    }
}