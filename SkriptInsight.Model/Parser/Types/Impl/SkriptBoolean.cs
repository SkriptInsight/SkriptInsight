using System;
using SkriptInsight.Model.Parser.Patterns;

namespace SkriptInsight.Model.Parser.Types.Impl
{
    [TypeDescription("boolean")]
    public class SkriptBoolean : SkriptGenericType<SkriptBoolean.SkriptRepresentation?>
    {
        [Flags]
        public enum SkriptRepresentation
        {
            None = 0,
            True = 1,
            False = 1 << 1,
            Yes = (1 << 2) | True,
            No = (1 << 3) | False,
            On = (1 << 4) | True,
            Off = (1 << 5) | False
        }


        static SkriptBoolean()
        {
            Pattern = SkriptPattern.ParsePattern("(1¦true|2¦false|5¦yes|10¦no|17¦on|34¦off)");
        }

        public static SkriptPattern Pattern { get; }

        protected override SkriptRepresentation? TryParse(ParseContext ctx)
        {
            var clone = ctx.Clone();
            var result = Pattern.Parse(clone);
            if (result.IsSuccess)
            {
                ctx.CurrentPosition = clone.CurrentPosition;
                return (SkriptRepresentation) result.ParseMark;
            }
            return null;
        }

        public override string AsString(SkriptRepresentation? obj)
        {
            return (obj ?? SkriptRepresentation.False).ToString().ToLowerInvariant();
        }
    }
}