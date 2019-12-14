using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace SkriptInsight.Host.Lsp
{
    public class ViewportChangedParams : TextDocumentIdentifier
    {
        public List<Range> Ranges { get; set; }
    }
}