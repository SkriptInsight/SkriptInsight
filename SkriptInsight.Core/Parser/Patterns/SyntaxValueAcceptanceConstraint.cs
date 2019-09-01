using System;

namespace SkriptInsight.Core.Parser.Patterns
{
    [Flags]
    public enum SyntaxValueAcceptanceConstraint
    {
        None = 0,
        LiteralsOnly = 1,
        NoLiterals = 1 << 1,
        VariablesOnly = 1 << 2,
        NullWhenOmitted = 1 << 3,
        AllowConditionalExpressions = 1 << 4,
    }

    public static class SyntaxValueAcceptanceConstraintExtensions
    {
        public static bool HasFlagFast(this SyntaxValueAcceptanceConstraint value, SyntaxValueAcceptanceConstraint flag)
        {
            return (value & flag) != 0;
        }
    }
}