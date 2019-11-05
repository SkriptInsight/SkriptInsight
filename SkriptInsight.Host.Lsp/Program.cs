using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DiscordRPC;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server;
using SkriptInsight.Core.Files;
using SkriptInsight.Core.Managers;
using SkriptInsight.Core.Parser.Patterns;
using SkriptInsight.Host.Lsp.Handlers;

namespace SkriptInsight.Host.Lsp
{
    internal static class Program
    {
        public static GoogleAnalyticsApi AnalyticsApi { get; set; }

        public static DiscordRpcClient DiscordRpcClient { get; set; }

        public static string EditorName { get; set; }

        private static async Task Main(string[] args)
        {
            StartAnalytics();
            AnalyticsApi.UserAgent = EditorName = GetEditorNameByArgs(args);
            
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
//                    .WithLoggerFactory(new LoggerFactory())
                    .AddDefaultLoggingProvider()
                    .WithHandler<TextDocumentHandler>()
//                    .WithMinimumLogLevel(LogLevel.Error)
                    .WithHandler<TextHoverHandler>()
                    .OnRequest<object, int>("insight/inspectionsCount", _ => Task.FromResult(0));
            });
            WorkspaceManager.CurrentHost = new LspSkriptInsightHost(server);
            Task.Run(() => StartDiscordRichPresence(WorkspaceManager.CurrentWorkspace));

            await server.WaitForExit;

            DiscordRpcClient?.Dispose();
            AnalyticsApi.CancellationToken.Cancel();
            AnalyticsApi.TrackEvent("ServerStop", "Stopped LSP Server");
            AnalyticsApi.TrackEvent("SessionStop", "Session Stop", extraValues: new {sc = "stop"});
        }

        private static void StartDiscordRichPresence(SkriptWorkspace workspace)
        {
            DiscordRpcClient = new DiscordRpcClient("635138726099419136", autoEvents: true);
            DiscordRpcClient.Initialize();

            DiscordRpcClient.SetPresence(new RichPresence
            {
                Assets = new Assets
                {
                    LargeImageKey = "logo-si-alpha2",
                    LargeImageText = "Using SkriptInsight",
                    SmallImageKey = EditorName.ToLower().Replace(" ", "_"),
                    SmallImageText = $"on {EditorName}"
                },
                Details = $"Idling on {EditorName}",
                State = "Developing SkriptInsight"
            });
        }

        private static string GetEditorNameByArgs(IEnumerable<string> args)
        {
            if (args.Contains("-vscode"))
                return "VSCode";

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