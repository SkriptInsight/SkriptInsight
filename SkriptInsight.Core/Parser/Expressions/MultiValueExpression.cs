using System.Collections.Generic;
using System.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Parser.Types;

namespace SkriptInsight.Core.Parser.Expressions
{
    public class MultiValueExpression : IExpression
    {
        public struct ValueDescription
        {
            public ValueDescription(IExpression expression, string splitter)
            {
                Expression = expression;
                Splitter = splitter ?? "";
            }

            public IExpression Expression { get; set; }

            public string Splitter { get; set; }

            public override string ToString()
            {
                return $"{Expression.AsString()}{(!Splitter.Equals(",") ? " " : "")}{Splitter}".TrimEnd();
            }
        }

        public object Value
        {
            get => Values;
            set => Values = value as List<ValueDescription>;
        }

        public List<ValueDescription> Values { get; set; } = new List<ValueDescription>();

        public ISkriptType Type { get; set; }

        public Range Range { get; set; }

        public ParseContext Context { get; set; }

        public string AsString()
        {
            return string.Join(" ", Values.Select(c => c.ToString()));
        }

        public override string ToString() => AsString();
    }
}