using System.Collections.Generic;
using System.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Parser.Types;

namespace SkriptInsight.Core.Parser.Expressions
{
    public class MultiValueExpression : IExpression
    {
        public class ValueDescription
        {
            public ValueDescription(IExpression expression, ParseMatch splitter)
            {
                Expression = expression;
                RawSplitter = splitter;
            }

            public IExpression Expression { get; set; }

            public string Splitter => RawSplitter?.RawContent.Trim() ?? "";
            
            public ParseMatch RawSplitter { get; set; }

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

        public List<MatchAnnotation> MatchAnnotations { get; set; } = new List<MatchAnnotation>();

        public override string ToString() => AsString();
    }
}