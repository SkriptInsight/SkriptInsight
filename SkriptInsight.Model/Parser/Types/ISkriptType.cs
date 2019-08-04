using JetBrains.Annotations;
using SkriptInsight.Model.Parser.Patterns;

namespace SkriptInsight.Model.Parser.Types
{
    [UsedImplicitly]
    public abstract class SkriptType<T> : ISkriptTypeBase
    {
        protected abstract Expression<T> ParseExpression(ParseContext ctx, SyntaxValueAcceptanceConstraint constraint);
        
        protected abstract string RenderExpression(Expression<T> value);
        
        public IExpression Parse(ParseContext ctx, SyntaxValueAcceptanceConstraint constraint)
        {
            return ParseExpression(ctx, SyntaxValueAcceptanceConstraint.None);
        }

        public string Render(IExpression value)
        {
            return RenderExpression(value as Expression<T>);
        }
    }
}