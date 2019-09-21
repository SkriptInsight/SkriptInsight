using System;
using SkriptInsight.Core.Parser.Expressions;

namespace SkriptInsight.Core.Parser.Types
{
    public abstract class SkriptGenericType<T> : ISkriptType
    {
        protected abstract T TryParse(ParseContext ctx);
        public abstract string AsString(T obj);

        public IExpression TryParseValue(ParseContext ctx)
        {
            ctx.StartRangeMeasure("Generic Type");
            var result = TryParse(ctx);
            if (result != null)
            {
                var expression = (IExpression) Activator.CreateInstance(
                    typeof(Expression<>).MakeGenericType(typeof(T)),
                    result,
                    ctx.EndRangeMeasure()
                );
                if (expression.Type == null)
                    expression.Type = this;
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