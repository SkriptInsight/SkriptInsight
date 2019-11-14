using System;
using System.Linq;
using SkriptInsight.Core.Parser.Patterns;
using SkriptInsight.Core.Parser.Patterns.Impl;
using static SkriptInsight.Core.Parser.Patterns.Impl.ChoicePatternElement;
using static SkriptInsight.Core.Parser.Types.Impl.SkriptBoolean.SkriptRepresentation;

namespace SkriptInsight.Core.Parser.Types.Impl
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
            Pattern = new SkriptPattern
            {
                Children =
                {
                    new ChoicePatternElement
                    {
                        Elements = Enum.GetValues(typeof(SkriptRepresentation)).Cast<SkriptRepresentation>()
                            .Where(c => c != None)
                            .Select(c =>
                                new ChoiceGroupElement(new LiteralPatternElement(c.ToString()),
                                    (int) c)).ToList()
                    }
                }
            };
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
            return (obj ?? False).ToString().ToLowerInvariant();
        }
    }
}