using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace SkriptInsight.Model.Parser.Types
{
    public class Expression<T> : IExpression
    {
        public Expression(T val, ParseMatch match, Range contentRange = null)
        {
            GenericValue = val;
            Match = match;
            Range = match?.Range;
            Context = match?.Context;
            ContentRange = contentRange ?? Range;
        }
        
        public object Value { get; set; }

        public T GenericValue
        {
            get => (T) Value;
            set => Value = value;
        }
        
        public Range Range { get; set; }
        
        public Range ContentRange { get; set; }
        
        public ParseContext Context { get; set; }
        
        public ParseMatch Match { get; set; }
    }
}