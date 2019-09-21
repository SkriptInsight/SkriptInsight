using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using SkriptInsight.Core.Parser.Expressions;

namespace SkriptInsight.Core.Extensions
{
    public static class ExpressionExtensions
    {
        public static IEnumerable<IExpression> GetValues<T>(this IExpression expr)
        {
            switch (expr)
            {
                case Parser.Expressions.Expression<T> genExpr:
                    yield return genExpr;
                    break;
                case MultiValueExpression multiVal:
                    foreach (var expression in multiVal.Values.SelectMany(valDesc => valDesc.Expression.GetValues<T>()))
                    {
                        yield return expression;
                    }
                    break;
                case ParenthesesExpression parExpr:
                    if (parExpr.InnerExpression == null) break;
                    
                    foreach (var expression in parExpr.InnerExpression.GetValues<T>()) yield return expression;
                    
                    break;
                default:
                    if (expr.Value is T)
                        yield return expr;
                    break;
            }
        }
    }
}