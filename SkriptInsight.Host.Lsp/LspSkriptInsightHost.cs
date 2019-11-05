using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using SkriptInsight.Core;
using ILanguageServer = OmniSharp.Extensions.LanguageServer.Server.ILanguageServer;

namespace SkriptInsight.Host.Lsp
{
    public class LspSkriptInsightHost : ISkriptInsightHost
    {
        public LspSkriptInsightHost(ILanguageServer server)
        {
            Server = server;
        }

        public ILanguageServer Server { get; set; }
        
        public void LogInfo(string message)
        {
            Server.Window.LogInfo(message);
        }

        public void PublishDiagnostics(Uri url, List<Diagnostic> diagnostics)
        {
            Server.Document.PublishDiagnostics(new PublishDiagnosticsParams
            {
                Uri = url,
                Diagnostics = diagnostics
            });
        }

        public bool SupportsRawRequests => true;

        public void SendRawNotification<T>(string name, T @params)
        {
            Server.SendNotification(name, @params);
        }

        public Task<TResponse> SendRawRequest<T, TResponse>(string method, T @params)
        {
            return Server.SendRequest<T, TResponse>(method, @params);
        }
    }
}