using System.Reflection;
using JetBrains.Annotations;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Parser;
using SkriptInsight.Core.Parser.Expressions;
using SkriptInsight.Core.Parser.Patterns;
using SkriptInsight.Core.Parser.Patterns.Impl;
using SkriptInsight.Core.Parser.Types.Impl;

namespace SkriptInsight.Core.Files.Nodes.Impl.Signatures.ControlFlow
{
    [SectionNode]
    public abstract class ConditionalBaseSignature<T> : SignatureFileNode<T>, IConditionalBaseSignature
        where T : ConditionalBaseSignature<T>, new()
    {
        protected ConditionalBaseSignature() : base(null)
        {
        }

        static ConditionalBaseSignature()
        {
            ExpressionPattern = new SkriptPattern();
            var conditionalBasePrefixAttribute = typeof(T).GetCustomAttribute<ConditionalBasePrefixAttribute>();
            var prefix = conditionalBasePrefixAttribute?.Prefix;
            if (!prefix.IsEmpty())
            {
                var pattern = new SkriptPattern
                {
                    Children =
                    {
                        new LiteralPatternElement(prefix),
                        new LiteralPatternElement(" ")
                    }
                };
                ExpressionPattern.Children.Add(conditionalBasePrefixAttribute?.IsOptional == true
                    ? (AbstractSkriptPatternElement) new OptionalPatternElement {Element = pattern}
                    : pattern);
            }

            ExpressionPattern.Children.Add(new TypePatternElement
            {
                Type = "boolean",
                NarrowMatch = false,
                Constraint = SyntaxValueAcceptanceConstraint.AllowConditionalExpressions
            });
        }

        private static SkriptPattern ExpressionPattern { get; set; }

        public ConditionalExpression Condition { get; set; }

        public SkriptExpression Expression { get; set; }

        public IExpression ConditionExpression => Condition ?? (IExpression) Expression ?? Literal;

        public Expression<SkriptBoolean.SkriptRepresentation?> Literal { get; set; }

        [CanBeNull]
        public static T TryParse(ParseContext ctx)
        {
            var parseContext = ctx.Clone(false);
            var parseResult = ExpressionPattern.Parse(parseContext);

            if (!parseResult.IsSuccess) return null;

            var literalExpression =
                parseResult.Matches.GetValue<SkriptBoolean.SkriptRepresentation?>(0) as
                    Expression<SkriptBoolean.SkriptRepresentation?>;

            //Check if literal
            T expression = null;

            if (literalExpression?.GenericValue != null)
            {
                expression = new T
                {
                    Literal = literalExpression
                };
            }

            //No literal found. Attempt to get a conditional expression
            if (parseResult.Matches.GetValue<ConditionalExpression>(0) is ConditionalExpression conditionalExpression)
            {
                expression = new T
                {
                    Condition = conditionalExpression
                };
            }

            //Users can also use expressions that return a boolean. Try that
            if (parseResult.Matches.GetValue<SkriptExpression>(0) is SkriptExpression skriptExpression)
            {
                expression = new T
                {
                    Expression = skriptExpression
                };
            }

            if (expression != null)
            {
                ctx.ReadUntilPosition(parseContext.CurrentPosition);
            }
            
            return expression;
        }

        protected ConditionalBaseSignature(T signature) : base(signature)
        {
        }
    }
}