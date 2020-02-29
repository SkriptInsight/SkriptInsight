using System;
using System.Collections.Generic;
using SkriptInsight.Core.Parser.Expressions;
using SkriptInsight.Core.Parser.Types.Impl;

namespace SkriptInsight.Core.Parser.Types
{
    public abstract class SkriptGenericType<T> : ISkriptType
    {
        protected virtual T TryParse(ParseContext ctx)
        {
            return default;
        }

        protected virtual T TryParse(ParseContext ctx, List<MatchAnnotation> matchAnnotationsHolder)
        {
            return default;
        }
        
        
        public abstract string AsString(T obj);

        public IExpression TryParseValue(ParseContext ctx)
        {
            var matchAnnotations = new List<MatchAnnotation>();
            
            ctx.StartRangeMeasure("Generic Type");
            var result = TryParse(ctx, matchAnnotations) ?? TryParse(ctx);
            if (result != null)
            {
                var expression = (IExpression) Activator.CreateInstance(
                    typeof(Expression<>).MakeGenericType(typeof(T)),
                    result,
                    ctx.EndRangeMeasure()
                );
                if (expression.Type == null)
                    expression.Type = this;
                
                expression.MatchAnnotations = matchAnnotations;
                return expression;
            }

            ctx.UndoRangeMeasure();
            return null;
        }

        public string AsString(object obj)
        {
            return AsString((T) obj);
        }
    }
}