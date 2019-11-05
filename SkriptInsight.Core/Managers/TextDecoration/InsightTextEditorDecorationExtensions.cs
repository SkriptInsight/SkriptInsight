using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace SkriptInsight.Core.Managers.TextDecoration
{
    public static class InsightTextEditorDecorationExtensions
    {
        public static Task<TextEditorDecorationType> CreateTextEditorDecorationType(this ISkriptInsightHost server,
            DecorationRenderOptions options)
        {
            return server
                .SendRawRequest<DecorationRenderOptions, TextEditorDecorationType>("insight/createDecoration", options);
        }

        public static void SetDecorations(this ISkriptInsightHost server, Uri uri,
            TextEditorDecorationType decorationType, IEnumerable<Range> ranges)
        {
            server.SendRawNotification("insight/setDecorations", new SetDecorationsParams(uri, decorationType, ranges));
        }
    }
}