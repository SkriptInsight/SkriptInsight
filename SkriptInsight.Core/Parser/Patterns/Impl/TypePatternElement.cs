using System;
using System.Linq;
using SkriptInsight.Core.Managers;
using SkriptInsight.Core.Parser.Expressions;
using SkriptInsight.Core.Parser.Types;
using SkriptInsight.Core.Parser.Types.Impl;
using SkriptInsight.Core.Parser.Types.Impl.Generic;

namespace SkriptInsight.Core.Parser.Patterns.Impl
{
    [GroupPatternElementInfo('%')]
    public class TypePatternElement : AbstractGroupPatternElement
    {
        public SyntaxValueAcceptanceConstraint Constraint { get; set; }

        public string Type { get; set; }

        public bool SkipParenthesis { get; set; }

        /// <summary>
        /// Can this type, when matching list values, match 'and' or 'or'
        /// </summary>
        public bool CanMatchListConjunctions { get; set; } = true;

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
            foreach (var typeRaw in Type.Split("/"))
            {
                var type = WorkspaceManager.Instance.KnownTypesManager.GetTypeByName(typeRaw);
                ISkriptType skriptTypeDescriptor = null;
                var isMultipleValues = typeRaw.EndsWith("s");
                if (isMultipleValues)
                {
                    type = WorkspaceManager.Instance.KnownTypesManager.GetTypeByName(typeRaw) ?? WorkspaceManager.Instance.KnownTypesManager.GetTypeByName(typeRaw.Substring(0, Type.Length - 1));

                    if (type != null) // We have a multiple value request. Hand over to GenericMultiValueType
                        skriptTypeDescriptor = new GenericMultiValueType(type, Constraint, CanMatchListConjunctions);
                }
                else
                {
                    skriptTypeDescriptor = type?.CreateNewInstance();
                }

                IExpression result = null;

                if (!Constraint.HasFlagFast(SyntaxValueAcceptanceConstraint.LiteralsOnly))
                {
                    //Try parsing a variable
                    var reference = new SkriptVariableReferenceType();
                    var ctxClone = ctx.Clone();
                    try
                    {
                        result = reference.TryParseValue(ctxClone);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    if (result != null) ctx.ReadUntilPosition(ctxClone.CurrentPosition);
                }

                var oldPos = ctx.CurrentPosition;
                if (result == null)
                    result = skriptTypeDescriptor?.TryParseValue(ctx);
                if (result == null && !SkipParenthesis)
                {
                    // Type descriptor wasn't able to parse the code. Push back and try with parentheses.
                    ctx.CurrentPosition = oldPos;
                
                    //Try parsing with parentheses first
                    var parenthesesType = new GenericParenthesesType(typeRaw);
                    result = parenthesesType.TryParseValue(ctx);
                }

                if (result != null)
                {
                    if (result.Type == null) result.Type = skriptTypeDescriptor;
                    result.Context = ctx;
                    var match = new ExpressionParseMatch(result);
                    ctx.Matches.Add(match);
                }

                return result != null ? ParseResult.Success(ctx) : ParseResult.Failure(ctx);
            }
            
            return ParseResult.Failure(ctx);
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