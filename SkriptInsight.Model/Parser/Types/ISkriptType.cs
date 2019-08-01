using SkriptInsight.Model.Parser.Patterns;

namespace SkriptInsight.Model.Parser.Types
{
    public abstract class SkriptType<T> : ISkriptTypeBase
    {
        protected abstract Expression<T> ParseExpression(ParseContext ctx, SyntaxValueAcceptanceConstraint constraint);
        
        public IExpression Parse(ParseContext ctx, SyntaxValueAcceptanceConstraint constraint)
        {
            return ParseExpression(ctx, SyntaxValueAcceptanceConstraint.None);
        }
    }
}