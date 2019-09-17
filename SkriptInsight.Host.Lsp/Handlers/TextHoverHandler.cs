using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Force.DeepCloner;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using SkriptInsight.Core.Files;
using SkriptInsight.Core.Managers;

namespace SkriptInsight.Host.Lsp.Handlers
{
    public class TextHoverHandler : IHoverHandler
    {
        public Task<Hover> Handle(HoverParams request, CancellationToken cancellationToken)
        {
            var file = WorkspaceManager.Instance.GetOrCreateByUri(request.TextDocument.Uri);

            var nodeAtLine = file.Nodes.ElementAtOrDefault((int) request.Position.Line);
            var targetNode = nodeAtLine.DeepClone();
            targetNode.File = null;
            if (targetNode.MatchedSyntax?.Result?.Context != null)
                targetNode.MatchedSyntax.Result.Context = null;

            return Task.FromResult(new Hover
            {
                Range = targetNode.ContentRange,
                Contents = new MarkedStringsOrMarkupContent(new MarkupContent
                {
                    Kind = MarkupKind.Markdown,
                    Value = $@"```js{"\n"}{JsonConvert.SerializeObject(targetNode, new JsonSerializerSettings
                    {
                        Converters = new List<JsonConverter> {new StringEnumConverter {NamingStrategy = new CamelCaseNamingStrategy()}},
                        ContractResolver = new NoFileContractResolver(),
                        Formatting = Formatting.Indented,
                        NullValueHandling = NullValueHandling.Ignore,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })}{"\n"}```"
                })
            });
        }

        public TextDocumentRegistrationOptions GetRegistrationOptions()
        {
            return new TextDocumentRegistrationOptions
            {
                DocumentSelector = SkriptFile.Selector
            };
        }

        public void SetCapability(HoverCapability capability)
        {
            capability.ContentFormat = new Container<MarkupKind>(MarkupKind.Markdown);
        }
    }
}