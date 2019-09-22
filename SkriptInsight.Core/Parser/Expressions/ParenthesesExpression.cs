using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Parser.Types;
using SkriptInsight.Core.Parser.Types.Impl;

namespace SkriptInsight.Core.Parser.Expressions
{
    public class ParenthesesExpression : IExpression
    {
        public ParenthesesExpression(IExpression innerExpression, ISkriptType type)
        {
            InnerExpression = innerExpression;
            Type = type;
        }

        public IExpression InnerExpression { get; set; }
        
        public object Value
        {
            get => InnerExpression;
            set => InnerExpression = (IExpression) value;
        }

        public ISkriptType Type { get; set; }
        
        public Range Range { get; set; }
        
        public ParseContext Context { get; set; }
        
        public string AsString()
        {
            return $"({InnerExpression?.AsString() ?? "<none>"})";
        }
    }
}