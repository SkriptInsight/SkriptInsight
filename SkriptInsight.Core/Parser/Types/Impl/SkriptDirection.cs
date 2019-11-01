using SkriptInsight.Core.Types;

namespace SkriptInsight.Core.Parser.Types.Impl
{
    [TypeDescription("direction")]
    public class SkriptDirection : SkriptGenericType<Direction>
    {
        protected override Direction TryParse(ParseContext ctx)
        {
            return null;
        }

        public override string AsString(Direction obj)
        {
            return obj.ToString();
        }
    }
}