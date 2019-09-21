using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace SkriptInsight.Core.Managers.TextDecoration
{
    public static class InsightTextEditorDecorationExtensions
    {
        public static Task<TextEditorDecorationType> CreateTextEditorDecorationType(this ILanguageServer server,
            DecorationRenderOptions options)
        {
            return server
                .SendRequest<DecorationRenderOptions, TextEditorDecorationType>("insight/createDecoration", options);
        }

        public static void SetDecorations(this ILanguageServer server, Uri uri,
            TextEditorDecorationType decorationType, IEnumerable<Range> ranges)
        {
            server.SendNotification("insight/setDecorations", new SetDecorationsParams(uri, decorationType, ranges));
        }
    }
}