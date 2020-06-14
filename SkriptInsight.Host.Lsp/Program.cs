using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DiscordRPC;
using OmniSharp.Extensions.LanguageServer.Server;
using SkriptInsight.Core.Files;
using SkriptInsight.Core.Managers;
using SkriptInsight.Core.Utils;
using SkriptInsight.Host.Lsp.Handlers;
using ThrottleDebounce;
using ILanguageServer = OmniSharp.Extensions.LanguageServer.Protocol.Server.ILanguageServer;

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
                while (!Debugger.IsAttached)
                    await Task.Delay(100);

            ILanguageServer server = null;
            server = await LanguageServer.From(options =>
            {
                options
                    .WithInput(Console.OpenStandardInput())
                    .WithOutput(Console.OpenStandardOutput())
                    .ConfigureLogging(c => c.AddLanguageProtocolLogging())
                    .WithHandler<TextDocumentHandler>()
                    .WithHandler<TextHoverHandler>();

                options.OnRequest<object, int>("insight/inspectionsCount",
                    _ => Task.FromResult(WorkspaceManager.Instance.InspectionsManager.CodeInspections.Count));
                var debouncedParseRange = Debouncer.Debounce<SkriptFile>(file => file.NotifyVisibleNodesRangeChanged(),
                    TimeSpan.FromMilliseconds(500));

                options.OnNotification<ViewportChangedParams>("insight/viewportRangeChanged", e =>
                {
                    var file = WorkspaceManager.CurrentWorkspace.WorkspaceManager.GetOrCreateByUri(e.Uri.ToUri());
                    if (file == null) return;

                    file.VisibleRanges = e.Ranges;
                    debouncedParseRange.Run(file);
                });

                options.OnNotification("insight/notifySupportsExtendedCapabilities", async () =>
                {
                    if (WorkspaceManager.CurrentHost == null) return;

                    var sendRequest =
                        WorkspaceManager.CurrentHost.SendRawRequest<ExtendedHostCapabilities>(
                            "insight/queryExtendedCapabilities");
                    if (sendRequest != null)
                    {
                        WorkspaceManager.CurrentHost.ExtendedCapabilities = await sendRequest;
                    }
                });
            });

            WorkspaceManager.CurrentHost = new LspSkriptInsightHost(server);
            WorkspaceManager.CurrentHost.LogInfo("SkriptInsight loaded successfully.");
            
            #pragma warning disable 4014
            Task.Run(() => StartDiscordRichPresence(WorkspaceManager.CurrentWorkspace));
            #pragma warning restore 4014

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
            AnalyticsApi = new GoogleAnalyticsApi();
        }
    }
}