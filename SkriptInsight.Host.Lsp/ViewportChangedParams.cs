using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace SkriptInsight.Host.Lsp
{
    public class ViewportChangedParams
    {
        public List<Range> Ranges { get; set; }

        [JsonIgnore] public DocumentUri Uri { get; set; }

        [JsonProperty("uri")]
        public string RawUri
        {
            get => Uri?.ToString();
            set
            {
                if (value != null) Uri = DocumentUri.From(value);
            }
        }
    }
}