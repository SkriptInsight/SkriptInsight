using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Utils;

namespace SkriptInsight.Core
{
    public interface ISkriptInsightHost
    {
        void LogInfo(string message);

        void PublishDiagnostics(Uri url, List<Diagnostic> diagnostics);
     
        bool SupportsRawRequests { get; }
        
        void SendRawNotification(string name);
        
        void SendRawNotification<T>(string name, T @params);
        
        Task<TResponse> SendRawRequest<T, TResponse>(string method, T @params);
        
        Task<TResponse> SendRawRequest<TResponse>(string method);

        public bool SupportsExtendedCapabilities { get; }
        
        [CanBeNull]
        ExtendedHostCapabilities ExtendedCapabilities { get; set; }
    }
}