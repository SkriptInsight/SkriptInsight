using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.WorkDone;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using SkriptInsight.Core;
using SkriptInsight.Core.Utils;
using ILanguageServer = OmniSharp.Extensions.LanguageServer.Protocol.Server.ILanguageServer;

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
            Server.TextDocument.PublishDiagnostics(new PublishDiagnosticsParams
            {
                Uri = url,
                Diagnostics = diagnostics
            });
        }

        public bool SupportsRawRequests => true;

        public void SendRawNotification(string name)
        {
            Server.SendNotification(name);
        }

        public void SendRawNotification<T>(string name, T @params)
        {
            Server.SendNotification(name, @params);
        }

        public Task<TResponse> SendRawRequest<T, TResponse>(string method, T @params)
        {
            return Server.SendRequest<T>(method, @params).Returning<TResponse>(CancellationToken.None);
        }

        public Task<TResponse> SendRawRequest<TResponse>(string method)
        {
            return Server.SendRequest(method).Returning<TResponse>(CancellationToken.None);
        }

        public bool SupportsExtendedCapabilities => ExtendedCapabilities != null;

        public ExtendedHostCapabilities ExtendedCapabilities { get; set; }
        
        public IProgressManager ProgressManager => Server.Client.ProgressManager;
        
        public IServerWorkDoneManager WorkDoneManager => Server.WorkDoneManager;
    }
}