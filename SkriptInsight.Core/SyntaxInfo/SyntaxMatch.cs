using SkriptInsight.Core.Parser.Patterns;

namespace SkriptInsight.Core.SyntaxInfo
{
    public class SyntaxMatch
    {
        public SyntaxMatch(ISyntaxElement element, ParseResult result)
        {
            Element = element;
            Result = result;
        }

        public ISyntaxElement Element { get; set; }

        public ParseResult Result { get; set; }
    }
}