using SkriptInsight.Model.Parser.Patterns;

namespace SkriptInsight.Model.Parser.Types
{
    public interface ISkriptTypeBase
    {
        IExpression Parse(ParseContext ctx, SyntaxValueAcceptanceConstraint constraint = SyntaxValueAcceptanceConstraint.None);
    }
}