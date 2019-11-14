using System;
using System.Diagnostics;
using System.Linq;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Parser.Expressions;
using SkriptInsight.Core.Parser.Functions;
using SkriptInsight.Core.Parser.Patterns;
using SkriptInsight.Core.Parser.Patterns.Impl;
using SkriptInsight.Core.SyntaxInfo;

namespace SkriptInsight.Core.Parser.Types.Impl.Internal
{
    [TypeDescription("si_func_param")]
    [InternalType]
    public class SkriptFunctionParameter : SkriptGenericType<FunctionParameter>
    {
        // %*si_func_param_name%[ ]:[ ]%*classinfo%
        private static SkriptPattern ParsePattern { get; }

        //  = %*type%
        private SkriptPattern GetDefaultValuePatternForType(SkriptType type)
        {
            return new SkriptPattern
            {
                Children = 
                {
                    new LiteralPatternElement(" "),
                    new LiteralPatternElement("="),
                    new LiteralPatternElement(" "),
                    new TypePatternElement
                    {
                        Type = type.FinalTypeName, Constraint = SyntaxValueAcceptanceConstraint.LiteralsOnly
                    }
                }
            };
        }

        static SkriptFunctionParameter()
        {
            ParsePattern = new SkriptPattern
            {
                Children =
                {
                    new TypePatternElement
                    {
                        Type = "si_func_param_name", Constraint = SyntaxValueAcceptanceConstraint.LiteralsOnly,
                        SkipParenthesis = true
                    },
                    new OptionalPatternElement {Element = new LiteralPatternElement(" ")},
                    new LiteralPatternElement(":"),
                    new OptionalPatternElement {Element = new LiteralPatternElement(" ")},
                    new TypePatternElement
                    {
                        Type = "classinfo", Constraint = SyntaxValueAcceptanceConstraint.LiteralsOnly,
                        SkipParenthesis = true
                    }
                }
            };
        }


        protected override FunctionParameter TryParse(ParseContext ctx)
        {
            var clone = ctx.Clone();
            var result = ParsePattern.Parse(clone);
            if (result.IsSuccess)
            {
                var expressions = result.Matches.OfType<ExpressionParseMatch>().Select(c => c.Expression).ToList();
                var name = expressions.Select(e => e.GetExplicitValue<string>(0)).FirstOrDefault(c => c != null);
                var type = expressions.Select(e => e.GetExplicitValue<SkriptType>(0)).FirstOrDefault(c => c != null);

                if (name != null && type != null)
                {
                    //Try to match an optional default value
                    clone.Matches.Clear();


                    var defValPattern = GetDefaultValuePatternForType(type.GenericValue);
                    var optResult = defValPattern.Parse(clone);
                    IExpression defVal = null;
                    if (optResult.IsSuccess)
                    {
                        defVal = optResult.Matches.OfType<ExpressionParseMatch>().FirstOrDefault()?.Expression;
                    }


                    ctx.CurrentPosition = clone.CurrentPosition;
                    //Return what we have
                    return new FunctionParameter
                    {
                        NameExpr = name,
                        TypeExpr = type,
                        DefaultValue = defVal
                    };
                }
            }

            return null;
        }

        public override string RenderAsString(FunctionParameter obj)
        {
            return obj.ToString();
        }
    }
}