using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Parser.Expressions;
using SkriptInsight.Core.Parser.Patterns;
using SkriptInsight.Core.Parser.Patterns.Impl;
using SkriptInsight.Core.SyntaxInfo;

namespace SkriptInsight.Core.Parser.Functions
{
    public class FunctionSignature
    {
        static FunctionSignature()
        {
            //function %*si_func_name%([%*si_func_params%])[ :: %*classinfo%]
            FunctionSignaturePattern = new SkriptPattern
            {
                Children =
                {
                    new LiteralPatternElement("function "),
                    new TypePatternElement
                    {
                        Type = "si_func_name", SkipParenthesis = true,
                        Constraint = SyntaxValueAcceptanceConstraint.LiteralsOnly
                    },
                    new LiteralPatternElement("("),
                    new OptionalPatternElement
                    {
                        Element = new TypePatternElement
                        {
                            Type = "si_func_params", SkipParenthesis = true,
                            Constraint = SyntaxValueAcceptanceConstraint.LiteralsOnly,
                            CanMatchListConjunctions = false //Just match list with commas and not with 'and' or 'or'
                        }
                    },
                    new LiteralPatternElement(")"),
                    new OptionalPatternElement
                    {
                        Element = new SkriptPattern
                        {
                            Children =
                            {
                                new LiteralPatternElement(" :: "),
                                new TypePatternElement {Type = "classinfo", SkipParenthesis = true}
                            }
                        }
                    }
                }
            };
        }

        public Expression<string> NameExpr { get; set; }

        public List<Expression<FunctionParameter>> ParametersExpr { get; set; }

        public Expression<SkriptType> ReturnTypeExpr { get; set; }

        public string Name
        {
            get => NameExpr.GenericValue;
            set => NameExpr.GenericValue = value;
        }
        
        public SkriptType ReturnType => ReturnTypeExpr.GenericValue;

        public List<FunctionParameter> Parameters => ParametersExpr.Select(c => c.GenericValue).ToList();

        public ParseResult ParseResult { get; set; }
        
        private static SkriptPattern FunctionSignaturePattern { get; }

        [CanBeNull]
        public static FunctionSignature TryParse(ParseContext ctx)
        {
            var clone = ctx.Clone();

            var result = FunctionSignaturePattern.Parse(clone);

            if (!result.IsSuccess) return null;
            
            ctx.ReadUntilPosition(clone.CurrentPosition);
            return new FunctionSignature
            {
                NameExpr = result.Matches.GetExplicitValue<string>(0),
                ParametersExpr = result.Matches.GetValues<FunctionParameter>()
                    .OfType<Expression<FunctionParameter>>().ToList(),
                ReturnTypeExpr = result.Matches.GetExplicitValue<SkriptType>(0) ?? SkriptType.VoidExpr,
                ParseResult = result
            };
        }

        public override string ToString()
        {
            return $"function {Name}({string.Join(", ", Parameters)}){(ReturnTypeExpr?.GenericValue != SkriptType.Void ? $" :: {ReturnTypeExpr?.GenericValue?.FinalTypeName}" : "")}";
        }
    }
}