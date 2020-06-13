using System;
using System.Reflection;
using JetBrains.Annotations;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Files.Nodes;
using SkriptInsight.Core.Parser.Expressions;
using SkriptInsight.Core.Parser.Patterns;
using SkriptInsight.Core.Parser.Patterns.Impl;
using SkriptInsight.Core.Parser.Types.Impl;
using SkriptInsight.Core.SyntaxInfo;

namespace SkriptInsight.Core.Parser.Signatures.ControlFlow
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
            var prefix = typeof(T).GetCustomAttribute<ConditionalBasePrefixAttribute>()?.Prefix;
            if (!prefix.IsEmpty())
            {
                ExpressionPattern.Children.Add(new LiteralPatternElement(prefix));
                ExpressionPattern.Children.Add(new LiteralPatternElement(" "));
            }

            ExpressionPattern.Children.Add(new TypePatternElement
            {
                Type = "boolean",
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
            if (literalExpression?.GenericValue != null)
            {
                var expression = new T
                {
                    Literal = literalExpression
                };

                return expression;
            }

            //No literal found. Attempt to get a conditional expression
            if (parseResult.Matches.GetValue<ConditionalExpression>(0) is ConditionalExpression conditionalExpression)
            {
                return new T
                {
                    Condition = conditionalExpression
                };
            }

            //Users can also use expressions that return a boolean. Try that
            if (parseResult.Matches.GetValue<SkriptExpression>(0) is SkriptExpression skriptExpression)
            {
                return new T
                {
                    Expression = skriptExpression
                };
            }

            return null;
        }

        protected ConditionalBaseSignature(T signature) : base(signature)
        {
        }
    }
}