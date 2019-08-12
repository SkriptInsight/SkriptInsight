using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using SkriptInsight.Model.Managers;
using SkriptInsight.Model.Parser.Expressions;
using SkriptInsight.Model.Parser.Patterns;
using SkriptInsight.Model.Parser.Patterns.Impl;

namespace SkriptInsight.Model.Parser.Types.Impl
{
    public class GenericMultiValueType : ISkriptType
    {
        public GenericMultiValueType(KnownTypesManager.KnownType type)
        {
            Type = type;
            var typeName = type.SkriptRepresentations.FirstOrDefault();
            NextValuePattern = CreateNextValuePatternForType(typeName);
        }

        public KnownTypesManager.KnownType Type { get; }

        public SkriptPattern NextValuePattern { get; }

        public IExpression TryParseValue(ParseContext ctx)
        {
            var typeInstance = Type.CreateNewInstance();
            var ourContext = ctx.Clone();
            var resultExpression = new MultiValueExpression();

            ctx.StartRangeMeasure();
            ctx.StartMatch();

            var count = 0;
            var isValid = true;
            while (isValid)
            {
                ourContext.Matches.Clear();

                var result = NextValuePattern.Parse(ourContext);
                isValid = result.IsSuccess;

                if (!isValid) continue;

                var expr = (ourContext.Matches.FirstOrDefault() as ExpressionParseMatch)?.Expression;
                if (expr != null)
                {
                    resultExpression.Values.Add(
                        new MultiValueExpression.ValueDescription(expr,
                            ourContext.Matches.Skip(1).FirstOrDefault()?.RawContent));
                }

                count++;
            }

            if (count > 0) ctx.ReadUntilPosition(ourContext.CurrentPosition);

            if (count > 0)
            {
                resultExpression.Range = ctx.EndRangeMeasure();
                resultExpression.Context = ctx;
                resultExpression.Type = typeInstance;
                    
                return resultExpression;
            }

            ctx.UndoRangeMeasure();
            ctx.UndoMatch();
            return null;
        }

        public string AsString(object obj)
        {
            var typeInstance = Type.CreateNewInstance();
            return typeInstance.AsString(obj);
        }

        private static SkriptPattern CreateNextValuePatternForType(string typeName)
        {
            //%type%[[ ](,|or|and)[ ]]
            return new SkriptPattern
            {
                Children =
                {
                    new TypePatternElement
                    {
                        Constraint = SyntaxValueAcceptanceConstraint.None,
                        Type = typeName
                    },
                    new OptionalPatternElement
                    {
                        Element = new SkriptPattern
                        {
                            Children =
                            {
                                new OptionalPatternElement
                                {
                                    Element = new LiteralPatternElement(" ")
                                },
                                new ChoicePatternElement
                                {
                                    Elements =
                                    {
                                        new LiteralPatternElement(","),
                                        new LiteralPatternElement("or"),
                                        new LiteralPatternElement("and")
                                    }
                                },
                                new OptionalPatternElement
                                {
                                    Element = new LiteralPatternElement(" ")
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}