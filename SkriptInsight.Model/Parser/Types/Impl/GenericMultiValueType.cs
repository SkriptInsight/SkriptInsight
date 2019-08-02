using System.Collections.Generic;
using System.Linq;
using SkriptInsight.Model.Managers;
using SkriptInsight.Model.Parser.Patterns;
using SkriptInsight.Model.Parser.Patterns.Impl;

namespace SkriptInsight.Model.Parser.Types.Impl
{
    public class GenericMultiValueType : ISkriptTypeBase
    {
        public KnownTypesManager.KnownType Type { get; }

        public GenericMultiValueType(KnownTypesManager.KnownType type)
        {
            Type = type;
            var typeName = type.SkriptRepresentations.FirstOrDefault();
            NextValuePattern = CreateNextValuePatternForType(typeName);
        }

        private static SkriptPattern CreateNextValuePatternForType(string typeName)
        {
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
                                    SaveChoice = false,
                                    Elements =
                                    {
                                        new LiteralPatternElement(","),
                                        new LiteralPatternElement("or"),
                                        new LiteralPatternElement("and"),
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

        public SkriptPattern NextValuePattern { get; set; }

        public IExpression Parse(ParseContext ctx, SyntaxValueAcceptanceConstraint constraint)
        {
            var typeInstance = Type.CreateNewInstance();
            var ourContext = ctx.Clone();

            ctx.StartRangeMeasure();
            ctx.StartMatch();

            var count = 0;
            var isValid = true;
            while (isValid)
            {
                var result = NextValuePattern.Parse(ourContext);
                isValid = result.IsSuccess;

                if (!isValid) continue;
                count++;
            }

            if (count > 0) ctx.ReadUntilPosition(ourContext.CurrentPosition);

            if (count > 0)
                return new Expression<List<IExpression>>(
                    ourContext.Matches
                        .Select(c => typeInstance.Parse(c.RawContent))
                        .Where(c => c != null)
                        .ToList(),
                    ctx.EndMatch(), ctx.EndRangeMeasure());

            ctx.UndoRangeMeasure();
            ctx.UndoMatch();
            return null;
        }
    }
}