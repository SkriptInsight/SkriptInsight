using System.Linq;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Parser.Expressions;
using SkriptInsight.Core.Parser.Patterns;
using SkriptInsight.Core.Parser.Patterns.Impl;
using SkriptInsight.Core.Types;

namespace SkriptInsight.Core.Parser.Types.Impl
{
    [TypeDescription("enchantment type")]
    public class SkriptEnchantmentType : SkriptGenericType<EnchantmentType>
    {
        //%enchantment%[ %number%]
        private static SkriptPattern TypePattern { get; }

        static SkriptEnchantmentType()
        {
            TypePattern = new SkriptPattern
            {
                Children =
                {
                    new TypePatternElement
                    {
                        Type = "enchantment"
                    },
                    new OptionalPatternElement
                    {
                        Element = new SkriptPattern
                        {
                            Children =
                            {
                                new LiteralPatternElement(" "),
                                new TypePatternElement("number")
                            }
                        }
                    }
                }
            };
        }

        protected override EnchantmentType TryParse(ParseContext ctx)
        {
            var clone = ctx.Clone(false);

            var result = TypePattern.Parse(clone);

            if (result.IsSuccess)
            {
                var expressions = result.Matches.OfType<ExpressionParseMatch>().Select(c => c.Expression).ToList();

                var enchantment = expressions.SelectMany(e => e.GetEnumValues<Enchantment>()).FirstOrDefault();
                var level = expressions.SelectMany(e => e.GetValues<double>()).Cast<Expression<double?>>().Select(c => c.GenericValue ?? 1d)
                    .DefaultIfEmpty(1).FirstOrDefault();

                ctx.ReadUntilPosition(clone.CurrentPosition);
                return new EnchantmentType(enchantment, expressions.SelectMany(e => e.GetValues<double>()).Any(), level);
            }


            return null;
        }

        public override string RenderAsString(EnchantmentType obj)
        {
            return obj.ToString();
        }
    }
}