using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Parser.Types;

namespace SkriptInsight.Core.Parser.Expressions
{
    public interface IExpression
    {
        object Value { get; set; }

        ISkriptType Type { get; set; }

        Range Range { get; set; }

        ParseContext Context { get; set; }

        string AsString();
    }
}