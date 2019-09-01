using System.Diagnostics;

namespace SkriptInsight.Core.Parser.Patterns
{
    [DebuggerDisplay("{" + nameof(RenderPattern) + "()}")]
    public abstract class AbstractSkriptPatternElement
    {
        public abstract ParseResult Parse(ParseContext ctx);

        public abstract string RenderPattern();

        public override string ToString()
        {
            return RenderPattern();
        }
    }
}