using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace SkriptInsight.Core.Parser
{
    
    /// <summary>
    /// Represents a parse match.
    /// </summary>
    public class ParseMatch
    {
        [JsonIgnore] public ParseContext Context { get; set; }

        public Range Range { get; set; }

        public string RawContent { get; set; }

        public override string ToString()
        {
            return RawContent;
        }
    }
}