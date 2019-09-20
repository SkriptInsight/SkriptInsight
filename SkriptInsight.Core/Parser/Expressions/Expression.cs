using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Parser.Types;

namespace SkriptInsight.Core.Parser.Expressions
{
    public class Expression<T> : IExpression
    {
        public Expression(T value)
        {
            GenericValue = value;
        }

        public Expression(T value, Range range)
        {
            GenericValue = value;
            Range = range;
        }

        public Expression()
        {
        }

        public T GenericValue
        {
            get => Value is T v ? v : default;
            set => Value = value;
        }

        public object Value { get; set; }

        public ISkriptType Type { get; set; }

        public Range Range { get; set; }

        public ParseContext Context { get; set; }

        public virtual string AsString()
        {
            return Type?.AsString(Value) ?? "";
        }
    }
}