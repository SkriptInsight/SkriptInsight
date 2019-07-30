using System.Diagnostics;

namespace SkriptInsight.Model.Parser.Patterns
{
    [DebuggerDisplay("{" + nameof(RenderPattern) + "()}")]
    public abstract class AbstractSkriptPatternElement
    {
        public PatternParseResult Parse(string pattern)
        {
            return Parse(ParseContext.FromCode(pattern));
        }

        public abstract PatternParseResult Parse(ParseContext ctx);

        public abstract string RenderPattern();

        public override string ToString()
        {
            return RenderPattern();
        }
    }
}