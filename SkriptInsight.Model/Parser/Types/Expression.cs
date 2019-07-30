using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace SkriptInsight.Model.Parser.Types
{
    public abstract class SkriptValue<T>
    {
        protected SkriptValue(ParseMatch match)
        {
            Range = match.Range;
            Context = match.Context;
        }

        public T Value { get; set; }

        public ISkriptType<T> Type { get; set; }

        public Range Range { get; set; }

        public ParseContext Context { get; set; }
    }
}