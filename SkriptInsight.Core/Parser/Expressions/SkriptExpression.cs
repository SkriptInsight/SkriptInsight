using System.Collections.Generic;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Parser.Types;
using SkriptInsight.Core.Parser.Types.Impl.Internal;
using SkriptInsight.Core.SyntaxInfo;

namespace SkriptInsight.Core.Parser.Expressions
{
    public class SkriptExpression : IExpression
    {
        public SkriptExpression(SyntaxSkriptExpression expression, Range range, ParseContext context)
        {
            Expression = expression;
            Range = range;
            Context = context;
            Type = InternalSkriptExpressionType.Instance;
        }
        
        public object Value { get; set; }

        public SyntaxSkriptExpression Expression
        {
            get => Value as SyntaxSkriptExpression;
            set => Value = value;
        }
        
        public ISkriptType Type { get; set; }
        
        public Range Range { get; set; }
        
        public ParseContext Context { get; set; }
        
        public string AsString()
        {
            return Context.Text.Substring((int) Range.Start.Character, (int) (Range.End.Character - Range.Start.Character));
        }

        public List<MatchAnnotation> MatchAnnotations { get; set; } = new List<MatchAnnotation>();
    }
}