using SkriptInsight.Core.Parser.Patterns;

namespace SkriptInsight.Core.SyntaxInfo
{
    public class SyntaxMatch
    {
        public SyntaxMatch(AbstractSyntaxElement element, ParseResult result)
        {
            Element = element;
            Result = result;
        }

        public AbstractSyntaxElement Element { get; set; }

        public ParseResult Result { get; set; }
    }
}