using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Files;
using SkriptInsight.Core.Managers;

namespace SkriptInsight.Host.Lsp.Handlers
{
    public class TextDocumentHandler : ITextDocumentSyncHandler
    {
        TextDocumentChangeRegistrationOptions IRegistration<TextDocumentChangeRegistrationOptions>.
            GetRegistrationOptions() =>
            new TextDocumentChangeRegistrationOptions
            {
                DocumentSelector = SkriptFile.Selector,
                SyncKind = TextDocumentSyncKind.Incremental
            };

        TextDocumentRegistrationOptions IRegistration<TextDocumentRegistrationOptions>.GetRegistrationOptions() =>
            new TextDocumentRegistrationOptions
            {
                DocumentSelector = SkriptFile.Selector
            };

        TextDocumentSaveRegistrationOptions IRegistration<TextDocumentSaveRegistrationOptions>.GetRegistrationOptions()
        {
            return new TextDocumentSaveRegistrationOptions
            {
                DocumentSelector = SkriptFile.Selector,
                IncludeText = false
            };
        }

        public TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri) =>
            new TextDocumentAttributes(uri, "skriptlang");

        public async Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken)
        {
            await Task.Yield();

            await Task.Run(() =>
            {
                var file = WorkspaceManager.Instance.GetOrCreateByUri(request.TextDocument.Uri.ToUri());
                foreach (var change in request.ContentChanges) file.HandleChange(change);
            }, cancellationToken);

            return Unit.Value;
        }

        public void SetCapability(SynchronizationCapability capability)
        {
        }

        public async Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
        {
            await Task.Yield();
            await Task.Run(() => Program.AnalyticsApi.TrackEvent("File-Open", "File opened"), cancellationToken);

            await Task.Run(
                () =>
                {
                    var file = WorkspaceManager.Instance.GetOrCreateByUri(request.TextDocument.Uri.ToUri());
                    WorkspaceManager.Instance.HandleOpenedFile(file);
                    file.RawContents.AddRange(request.TextDocument.Text.SplitOnNewLines());
                    file.PrepareNodes();
                },
                cancellationToken);

            return Unit.Value;
        }

        public async Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken)
        {
            await Task.Yield();
            await Task.Run(() => Program.AnalyticsApi.TrackEvent("File-Close", "File closed"), cancellationToken);

            await Task.Run(
                () => WorkspaceManager.Instance.HandleClosedFile(
                    WorkspaceManager.Instance.GetOrCreateByUri(request.TextDocument.Uri.ToUri())), cancellationToken);

            return Unit.Value;
        }

        public async Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken)
        {
            await Task.Yield();
            await Task.Run(() => Program.AnalyticsApi.TrackEvent("File-Close", "File closed"), cancellationToken);

            return Unit.Value;
        }
    }
}