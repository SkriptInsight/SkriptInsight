using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace SkriptInsight.Model.Parser.Types
{
    public interface IExpression
    {
        object Value { get; set; }

        Range Range { get; set; }

        Range ContentRange { get; set; }

        ParseContext Context { get; set; }

        ParseMatch Match { get; set; }
    }
}