using System;
using System.Collections.Generic;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace SkriptInsight.Core
{
    public interface ISkriptInsightHost
    {
        void LogInfo(string message);


        void PublishDiagnostics(Uri url, List<Diagnostic> diagnostics);
    }
}