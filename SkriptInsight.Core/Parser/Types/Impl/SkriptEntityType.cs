using System;
using System.Linq;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Parser.Expressions;
using SkriptInsight.Core.Parser.Patterns;
using SkriptInsight.Core.Parser.Patterns.Impl;
using SkriptInsight.Core.Types;

namespace SkriptInsight.Core.Parser.Types.Impl
{
    [TypeDescription("entitytype")]
    public class SkriptEntityType : SkriptGenericType<EntityType>
    {
        //[%number% ]%entitydata%
        private static SkriptPattern ParsePattern { get; }

        static SkriptEntityType()
        {
            ParsePattern = new SkriptPattern
            {
                Children =
                {
                    new OptionalPatternElement
                    {
                        Element = new SkriptPattern
                        {
                            Children =
                            {
                                new TypePatternElement
                                {
                                    Type = "number"
                                },
                                new LiteralPatternElement(" ")
                            }
                        }
                    },
                    new TypePatternElement
                    {
                        Type = "entitydata"
                    }
                }
            };
        }


        protected override EntityType TryParse(ParseContext ctx)
        {
            var clone = ctx.Clone();

            var result = ParsePattern.Parse(clone);

            if (result.IsSuccess)
            {
                var expressions = result.Matches.OfType<ExpressionParseMatch>().Select(c => c.Expression).ToList();

                var entityData = expressions.SelectMany(e => e.GetValues<EntityData>()).FirstOrDefault()?.Value as EntityData;
                var amount = expressions.SelectMany(e => e.GetValues<double>()).Cast<Expression<double?>>()
                    .Select(c => c.GenericValue ?? 1d)
                    .DefaultIfEmpty(1).FirstOrDefault();
                if (entityData == null) return null;
                
                ctx.CurrentPosition = clone.CurrentPosition;
                return new EntityType(amount, expressions.SelectMany(e => e.GetValues<double>()).Any(), entityData);
            }

            return null;
        }

        public override string AsString(EntityType obj)
        {
            return obj.ToString();
        }
    }
}