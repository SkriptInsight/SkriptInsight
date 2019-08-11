using SkriptInsight.Model.Extensions;
using SkriptInsight.Model.Parser.Expressions;

namespace SkriptInsight.Model.Parser
{
    public class ExpressionParseMatch : ParseMatch
    {
        public ExpressionParseMatch(IExpression expression)
        {
            Expression = expression;
            Range = expression.Range;
            
            var lines = expression.Context.Text.SplitOnNewLines();
            var startPos = Range.Start.ResolveFor(lines);
            var endPos = Range.End.ResolveFor(lines);
            RawContent = expression.Context.Text.Substring(startPos, endPos - startPos);
        }

        public IExpression Expression { get; set; }
    }
}