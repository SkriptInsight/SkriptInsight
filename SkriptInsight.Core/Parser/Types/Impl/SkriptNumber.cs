using System;
using System.Globalization;
using System.Linq;
using System.Text;
using SkriptInsight.Core.Parser.Patterns;
using SkriptInsight.Core.Parser.Patterns.Impl;
using static SkriptInsight.Core.Parser.Patterns.Impl.ChoicePatternElement;

namespace SkriptInsight.Core.Parser.Types.Impl
{
    [TypeDescription("number")]
    public class SkriptNumber : SkriptGenericType<double?>
    {
        private const int Dot = 11;
        private const int Minus = 12;

        public static SkriptPattern NumberPattern { get; }

        static SkriptNumber()
        {
            //(0¦0|1¦1|2¦2|3¦3|4¦4|5¦5|6¦6|7¦7|8¦8|9¦9|11¦.|12¦.)
            NumberPattern = new SkriptPattern
            {
                Children =
                {
                    new ChoicePatternElement
                    {
                        Elements =
                        {
                            new ChoiceGroupElement(new LiteralPatternElement("0")),
                            new ChoiceGroupElement(new LiteralPatternElement("1"), 1),
                            new ChoiceGroupElement(new LiteralPatternElement("2"), 2),
                            new ChoiceGroupElement(new LiteralPatternElement("3"), 3),
                            new ChoiceGroupElement(new LiteralPatternElement("4"), 4),
                            new ChoiceGroupElement(new LiteralPatternElement("5"), 5),
                            new ChoiceGroupElement(new LiteralPatternElement("6"), 6),
                            new ChoiceGroupElement(new LiteralPatternElement("7"), 7),
                            new ChoiceGroupElement(new LiteralPatternElement("8"), 8),
                            new ChoiceGroupElement(new LiteralPatternElement("9"), 9),
                            new ChoiceGroupElement(new LiteralPatternElement("."), Dot),
                            new ChoiceGroupElement(new LiteralPatternElement("-"), Minus)
                        }
                    }
                }
            };
        }

        protected override double? TryParse(ParseContext ctx)
        {
            var ctxClone = ctx.Clone(false);

            var sb = new StringBuilder();
            var hasDot = false;
            var negative = false;
            var isValid = true;
            while (isValid)
            {
                var parseResult = NumberPattern.Parse(ctxClone);

                if (parseResult.IsSuccess)
                {
                    if (parseResult.ParseMark == Dot)
                    {
                        if (!hasDot)
                            hasDot = true;
                        else
                            isValid = false;
                    }
                    else if (parseResult.ParseMark == Minus)
                    {
                        if (!negative && sb.Length == 0)
                            negative = true;
                        else
                            isValid = false;
                        
                        continue;
                    }
                    
                    sb.Append(parseResult.Matches.Last().RawContent);
                }
                else isValid = false;
            }

            if (!double.TryParse(sb.ToString(), out var result)) return null;

            ctx.ReadUntilPosition(ctxClone.CurrentPosition);
            
            return Math.Abs(result) * (negative ? -1 : 1);
        }

        public override string AsString(double? obj)
        {
            return obj?.ToString(CultureInfo.InvariantCulture) ?? "<none>";
        }
    }
}