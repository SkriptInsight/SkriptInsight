using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
            if (nodeAtLine != null)
            {
                return Task.FromResult(new Hover
                {
                    Range = nodeAtLine.ContentRange,
                    Contents = new MarkedStringsOrMarkupContent(new MarkupContent
                    {
                        Kind = MarkupKind.Markdown,
                        Value = $@"```js{"\n"}{JsonConvert.SerializeObject(nodeAtLine, new JsonSerializerSettings
                        {
                            Formatting = Formatting.Indented,
                            NullValueHandling = NullValueHandling.Ignore
                        })}{"\n"}```"
                    })
                });
            }

            return null;
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