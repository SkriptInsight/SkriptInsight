using System;
using System.Linq;
using SkriptInsight.Core.Managers;
using SkriptInsight.Core.Parser.Expressions;
using SkriptInsight.Core.Parser.Expressions.Variables;
using SkriptInsight.Core.Parser.Types;
using SkriptInsight.Core.Parser.Types.Impl;

namespace SkriptInsight.Core.Parser.Patterns.Impl
{
    [GroupPatternElementInfo('%')]
    public class TypePatternElement : AbstractGroupPatternElement
    {
        public SyntaxValueAcceptanceConstraint Constraint { get; set; }

        public string Type { get; set; }

        public TypePatternElement(string contents) : base(contents)
        {
            var list = contents.TakeWhile(c => ParseConstraint(c) != SyntaxValueAcceptanceConstraint.None)
                .Select(ParseConstraint).ToList();

            Type = contents.Substring(list.Count);
            if (!list.Any()) list.Add(SyntaxValueAcceptanceConstraint.None);
            Constraint = list.Aggregate((a, b) => a | b);
        }

        public TypePatternElement()
        {
        }

        public override ParseResult Parse(ParseContext ctx)
        {
            var type = KnownTypesManager.Instance.GetTypeByName(Type);
            ISkriptType skriptTypeDescriptor = null;

            if (Type.EndsWith("s"))
            {
                type = KnownTypesManager.Instance.GetTypeByName(Type.Substring(0, Type.Length - 1));

                if (type != null) // We have a multiple value request. Hand over to GenericMultiValueType
                    skriptTypeDescriptor = new GenericMultiValueType(type);
            }
            else
            {
                skriptTypeDescriptor = type?.CreateNewInstance();
            }

            IExpression result = null;

            if (!Constraint.HasFlagFast(SyntaxValueAcceptanceConstraint.LiteralsOnly))
            {
                //Try parsing a variable first
                var reference = new SkriptVariableReference();
                var ctxClone = ctx.Clone();
                result = reference.TryParseValue(ctxClone);
                if (result != null) ctx.ReadUntilPosition(ctxClone.CurrentPosition);
            }

            if (result == null)
                result = skriptTypeDescriptor?.TryParseValue(ctx);

            if (result != null)
            {
                result.Type = skriptTypeDescriptor;
                result.Context = ctx;
                var match = new ExpressionParseMatch(result);
                ctx.Matches.Add(match);
            }

            return result != null ? ParseResult.Success(ctx) : ParseResult.Failure(ctx);
        }

        #region Constraint

        private static string RenderConstraint(SyntaxValueAcceptanceConstraint c) =>
            c switch
            {
                SyntaxValueAcceptanceConstraint.None => "",
                SyntaxValueAcceptanceConstraint.LiteralsOnly => "*",
                SyntaxValueAcceptanceConstraint.NoLiterals => "~",
                SyntaxValueAcceptanceConstraint.VariablesOnly => "^",
                SyntaxValueAcceptanceConstraint.NullWhenOmitted => "-",
                SyntaxValueAcceptanceConstraint.AllowConditionalExpressions => "=",
                _ => throw new ArgumentOutOfRangeException()
            };

        private static SyntaxValueAcceptanceConstraint ParseConstraint(char c) =>
            c switch
            {
                '*' => SyntaxValueAcceptanceConstraint.LiteralsOnly,
                '~' => SyntaxValueAcceptanceConstraint.NoLiterals,
                '^' => SyntaxValueAcceptanceConstraint.VariablesOnly,
                '-' => SyntaxValueAcceptanceConstraint.NullWhenOmitted,
                '=' => SyntaxValueAcceptanceConstraint.AllowConditionalExpressions,
                _ => SyntaxValueAcceptanceConstraint.None
            };

        #endregion

        public string ConstraintAsString => string.Join("", Enum.GetValues(typeof(SyntaxValueAcceptanceConstraint))
            .Cast<SyntaxValueAcceptanceConstraint>()
            .Where(c => Constraint.HasFlagFast(c))
            .Select(RenderConstraint));

        public override string RenderPattern()
        {
            return $"%{ConstraintAsString}{Type}%";
        }
    }
}