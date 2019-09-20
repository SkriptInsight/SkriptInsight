using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server;
using SkriptInsight.Core.Managers;
using SkriptInsight.Core.Parser.Patterns;
using SkriptInsight.Host.Lsp.Handlers;

namespace SkriptInsight.Host.Lsp
{
    internal static class Program
    {
        public static GoogleAnalyticsApi AnalyticsApi { get; set; }

        private static async Task Main(string[] args)
        {
            StartAnalytics();
            AnalyticsApi.UserAgent = GetEditorNameByArgs(args);

            AnalyticsApi.TrackEvent("SessionStart", "Session Start", extraValues: new {sc = "start"});
            AnalyticsApi.TrackEvent("ServerStart", "Started LSP Server", extraValues: new {sc = "start"});

            if (args.Any(a => a.ToLower().Equals("-d")))
            {
                while (!System.Diagnostics.Debugger.IsAttached)
                {
                    await Task.Delay(100);
                }
            }

            var server = await LanguageServer.From(options =>
            {
                options
                    .WithInput(Console.OpenStandardInput())
                    .WithOutput(Console.OpenStandardOutput())
                    .WithLoggerFactory(new LoggerFactory())
                    .AddDefaultLoggingProvider()
                    .WithHandler<TextDocumentHandler>()
                    .WithMinimumLogLevel(LogLevel.Error)
                    .WithHandler<TextHoverHandler>()
                    .OnRequest<object, int>("insight/inspectionsCount", _ => Task.FromResult(0));
            });
            WorkspaceManager.Instance.Current.Server = server;
            
            await server.WaitForExit;

            AnalyticsApi.CancellationToken.Cancel();
            AnalyticsApi.TrackEvent("ServerStop", "Stopped LSP Server");
            AnalyticsApi.TrackEvent("SessionStop", "Session Stop", extraValues: new {sc = "stop"});
        }

        private static string GetEditorNameByArgs(IEnumerable<string> args)
        {
            if (args.Contains("-vscode"))
                return "Visual Studio Code";

            return "Unknown";
        }

        private static void StartAnalytics()
        {
            AnalyticsApi = new GoogleAnalyticsApi
            {
                /*DisableTracking = true*/
            };
        }
    }
}