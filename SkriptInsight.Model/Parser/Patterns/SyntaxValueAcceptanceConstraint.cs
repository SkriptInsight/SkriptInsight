using System;

namespace SkriptInsight.Model.Parser.Patterns
{
    public enum SyntaxValueAcceptanceConstraint
    {
        None,
        LiteralsOnly,
        NoLiterals,
        VariablesOnly,
        NullWhenOmitted
    }
}