using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core;
using SkriptInsight.Core.Utils;
using Xunit.Sdk;

namespace SkriptInsight.Tests
{
    public class TestInsightHost : ISkriptInsightHost
    {
        public void LogInfo(string message)
        {
            Console.WriteLine(message);
        }

        public void PublishDiagnostics(Uri url, List<Diagnostic> diagnostics)
        {
            Console.WriteLine($"Got {diagnostics.Count} diagnostic(s) on {url}.");
        }

        public bool SupportsRawRequests => false;

        public void SendRawNotification(string name)
        {
            throw new FalseException("This shouldn't be invoked", false);
        }

        public void SendRawNotification<T>(string name, T @params)
        {
            throw new FalseException("This shouldn't be invoked", false);
        }

        public Task<TResponse> SendRawRequest<T, TResponse>(string method, T @params)
        {
            throw new FalseException("This shouldn't be invoked", false);
        }

        public Task<TResponse> SendRawRequest<TResponse>(string method)
        {
            throw new FalseException("This shouldn't be invoked", false);
        }

        public bool SupportsExtendedCapabilities => false;

        public ExtendedHostCapabilities ExtendedCapabilities { get; set; }
    }
}