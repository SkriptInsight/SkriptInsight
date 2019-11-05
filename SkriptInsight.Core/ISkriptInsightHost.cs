using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Managers.TextDecoration;

namespace SkriptInsight.Core
{
    public interface ISkriptInsightHost
    {
        void LogInfo(string message);

        void PublishDiagnostics(Uri url, List<Diagnostic> diagnostics);
     
        bool SupportsRawRequests { get; }
        
        void SendRawNotification<T>(string name, T @params);
        
        Task<TResponse> SendRawRequest<T, TResponse>(string method, T @params);
    }
}