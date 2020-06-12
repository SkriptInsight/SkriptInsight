#nullable enable
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Managers;
using SkriptInsight.Core.Parser.Expressions;
using SkriptInsight.Core.Parser.Patterns;
using SkriptInsight.Core.Parser.Patterns.Impl;
using static SkriptInsight.Core.Parser.Patterns.Impl.ChoicePatternElement;

namespace SkriptInsight.Core.Parser.Types.Impl.Generic
{
    public class GenericMultiValueType : ISkriptType
    {
        public GenericMultiValueType(KnownTypesManager.KnownType type,
            SyntaxValueAcceptanceConstraint constraint = SyntaxValueAcceptanceConstraint.None, bool isListValue = true)
        {
            Type = type;
            var typeName = type.SkriptRepresentations.FirstOrDefault();
            NextValuePattern = CreateNextValuePatternForType(typeName, constraint, isListValue);
        }

        public KnownTypesManager.KnownType Type { get; }

        public SkriptPattern NextValuePattern { get; }

        public IExpression? TryParseValue(ParseContext ctx)
        {
            var typeInstance = Type.CreateNewInstance();
            var ourContext = ctx.Clone(false);
            var resultExpression = new MultiValueExpression();

            ctx.StartRangeMeasure();
            ctx.StartMatch();

            var count = 0;
            var isValid = true;
            var lastGoodPos = ourContext.CurrentPosition;

            while (isValid)
            {
                ourContext.Matches.Clear();
                ourContext.RemoveNarrowLimit();

                var result = NextValuePattern.Parse(ourContext);
                isValid = result.IsSuccess;

                if (!isValid) continue;

                //Check if the parser consumed content and had a valid result. If not, just let go and give up.
                if (lastGoodPos == ourContext.CurrentPosition) break;

                var expr = (ourContext.Matches.FirstOrDefault() as ExpressionParseMatch)?.Expression;
                if (expr != null)
                    resultExpression.Values.Add(
                        new MultiValueExpression.ValueDescription(expr,
                            ourContext.Matches.Where(c => c != null && !(c is ExpressionParseMatch))
                                .Where(c => !c.RawContent.IsEmpty()).Skip(1).FirstOrDefault()));

                count++;
            }

            if (count > 0)
            {
                //Remove last splitter
                var lastVal = resultExpression.Values?.Last();
                if (lastVal != null)
                {
                    if (lastVal.RawSplitter != null)
                    {
                        ourContext.CurrentPosition = (int) lastVal.RawSplitter.Range.Start.Character;
                        lastVal.RawSplitter = null;
                    }

                    ctx.ReadUntilPosition(ourContext.CurrentPosition);
                }
            }

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
            return string.Empty;
        }

        private static SkriptPattern CreateNextValuePatternForType(string? typeName,
            SyntaxValueAcceptanceConstraint constraint, bool isListValue)
        {
            //%type%[[ ](,|or|and)[ ]]
            return new SkriptPattern
            {
                Children =
                {
                    new TypePatternElement
                    {
                        Constraint = constraint,
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
                                        new List<ChoiceGroupElement>
                                        {
                                            new LiteralPatternElement(",")
                                        }.Concat(
                                            isListValue
                                                ? new ChoiceGroupElement[]
                                                {
                                                    new LiteralPatternElement("or"),
                                                    new LiteralPatternElement("and")
                                                }
                                                : Enumerable.Empty<ChoiceGroupElement>()).ToList()
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