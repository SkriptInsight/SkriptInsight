using SkriptInsight.Model.Parser.Patterns;

namespace SkriptInsight.Model.Parser.Types
{
    public abstract class SkriptType<T> : ISkriptTypeBase
    {
        protected abstract Expression<T> ParseExpression(ParseContext ctx);
        
        public IExpression Parse(ParseContext ctx)
        {
            return ParseExpression(ctx);
        }
    }
}