using SkriptInsight.Model.Parser.Types;

namespace SkriptInsight.Model.Parser
{
    public class ExpressionParseMatch : ParseMatch
    {
        public ExpressionParseMatch(IExpression expression)
        {
            Expression = expression;
        }

        public IExpression Expression { get; set; }
    }
}