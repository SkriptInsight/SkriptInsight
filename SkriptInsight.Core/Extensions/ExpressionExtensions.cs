using System;
using System.Collections.Generic;
using System.Linq;
using SkriptInsight.Core.Parser.Expressions;
using SkriptInsight.Core.Types;

namespace SkriptInsight.Core.Extensions
{
    public static class ExpressionExtensions
    {
        public static IEnumerable<SkriptEnumValue<T>> GetEnumValues<T>(this IExpression expr) where T: Enum
        {
            return expr.GetValues<T>().Cast<Expression<SkriptEnumValue<T>>>().Select(val => val.GenericValue);
        }
        public static IEnumerable<T> GetSimpleEnumValues<T>(this IExpression expr) where T: Enum
        {
            return expr.GetValues<T>().Cast<Expression<SkriptEnumValue<T>>>().Select(val => val.GenericValue.Value);
        }
        
        public static IEnumerable<IExpression> GetValues<T>(this IExpression expr)
        {
            if (typeof(T).IsSubclassOf(typeof(Enum)))
            {
                foreach (var expression in expr.GetValues<SkriptEnumValue<T>>())
                {
                    yield return expression;
                }

                yield break;
            }
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
        
        public static IEnumerable<IExpression> GetAllExpressions(this IExpression expr)
        {
            switch (expr)
            {
                case MultiValueExpression multiVal:
                    foreach (var expression in multiVal.Values.SelectMany(valDesc => valDesc.Expression.GetAllExpressions()))
                    {
                        yield return expression;
                    }
                    break;
                case ParenthesesExpression parExpr:
                    if (parExpr.InnerExpression == null) break;
                    
                    foreach (var expression in parExpr.InnerExpression.GetAllExpressions()) yield return expression;
                    
                    break;
                default:
                    yield return expr;
                    break;
            }
        }
    }
}