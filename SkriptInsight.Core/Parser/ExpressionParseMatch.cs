using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Parser.Expressions;

namespace SkriptInsight.Core.Parser
{
    public class ExpressionParseMatch : ParseMatch
    {
        public ExpressionParseMatch(IExpression expression)
        {
            Expression = expression;
            Range = expression.Range;
            Context = expression.Context;
            
            var lines = expression.Context.Text.SplitOnNewLines();
            var startPos = Range.Start.ResolveFor(lines);
            var endPos = Range.End.ResolveFor(lines);
            RawContent = expression.Context.Text.Substring(startPos, endPos - startPos);
        }

        public IExpression Expression { get; set; }
    }
}