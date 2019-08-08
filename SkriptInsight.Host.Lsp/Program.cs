using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Server;

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
                options
                    .WithInput(Console.OpenStandardInput())
                    .WithOutput(Console.OpenStandardOutput())
                    .WithLoggerFactory(new LoggerFactory())
                    .AddDefaultLoggingProvider()
                    .WithMinimumLogLevel(LogLevel.Error)
                    .OnRequest<object, int>("insight/inspectionsCount", _ => Task.FromResult(12))
            );

            await server.WaitForExit;

            AnalyticsApi.CancellationToken.Cancel();
            AnalyticsApi.TrackEvent("ServerStop", "Stopped LSP Server");
            AnalyticsApi.TrackEvent("SessionStop", "Session Stop", extraValues: new {sc = "stop"});
        }

        private static string GetEditorNameByArgs(string[] args)
        {
            if (args.Contains("-vscode"))
                return "Visual Studio Code";

            return "Unknown";
        }

        private static void StartAnalytics()
        {
            AnalyticsApi = new GoogleAnalyticsApi {DisableTracking = true};
        }
    }
}