using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace SkriptInsight.Model.Parser
{
    public class ParseMatch
    {
        [JsonIgnore]
        public ParseContext Context { get; set; }

        public Range Range { get; set; }

        public string RawContent { get; set; }
    }
}