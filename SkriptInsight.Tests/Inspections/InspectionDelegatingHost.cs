using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.WorkDone;
using SkriptInsight.Core;
using SkriptInsight.Core.Utils;

namespace SkriptInsight.Tests.Inspections
{
    public class InspectionDelegatingHost : ISkriptInsightHost
    {
        public Action<Uri, List<Diagnostic>> DiagnosticHandler { get; set; }

        public InspectionDelegatingHost(Action<Uri, List<Diagnostic>> diagnosticHandler)
        {
            DiagnosticHandler = diagnosticHandler;
        }

        public void LogInfo(string message)
        {
        }

        public void PublishDiagnostics(Uri url, List<Diagnostic> diagnostics)
        {
            DiagnosticHandler(url, diagnostics);
        }

        public bool SupportsRawRequests => false;

        public void SendRawNotification(string name)
        {
            throw new System.NotImplementedException();
        }

        public void SendRawNotification<T>(string name, T @params)
        {
            throw new System.NotImplementedException();
        }

        public Task<TResponse> SendRawRequest<T, TResponse>(string method, T @params)
        {
            throw new System.NotImplementedException();
        }

        public Task<TResponse> SendRawRequest<TResponse>(string method)
        {
            throw new System.NotImplementedException();
        }

        public bool SupportsExtendedCapabilities => false;
        public ExtendedHostCapabilities ExtendedCapabilities { get; set; }
        public IProgressManager ProgressManager { get; }
        public IServerWorkDoneManager WorkDoneManager { get; }
    }
}