using System;
using SkriptInsight.Model.Parser.Expressions;

namespace SkriptInsight.Model.Parser.Types
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
                return (IExpression) Activator.CreateInstance(
                    typeof(Expression<>).MakeGenericType(typeof(T)),
                    result,
                    ctx.EndRangeMeasure()
                );
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