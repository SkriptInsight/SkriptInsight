using System;
using System.Linq;
using SkriptInsight.Model.Managers;

namespace SkriptInsight.Model.Parser.Patterns.Impl
{
    [GroupPatternElementInfo('%')]
    public class TypePatternElement : AbstractGroupPatternElement
    {
        public SyntaxValueAcceptanceConstraint Constraint { get; set; }

        public string Type { get; }

        public TypePatternElement(string contents) : base(contents)
        {
            var list = contents.TakeWhile(c => ParseConstraint(c) != SyntaxValueAcceptanceConstraint.None)
                .Select(ParseConstraint).ToList();
            
            Type = contents.Substring(list.Count);
            if (!list.Any()) list.Add(SyntaxValueAcceptanceConstraint.None);
            Constraint = list.Aggregate((a, b) => a | b);
        }

        public override ParseResult Parse(ParseContext ctx)
        {
            var type = KnownTypesManager.Instance.GetTypeByName(Type);
            var result = type?.CreateNewInstance()?.Parse(ctx);

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