using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Files;
using SkriptInsight.Core.Managers;
using Xunit;
using Xunit.Sdk;

namespace SkriptInsight.Tests
{
    public class SkriptFileStructureTests
    {
        private class TestInsightHost : ISkriptInsightHost
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
        }

        public SkriptFileStructureTests()
        {
            WorkspaceManager.CurrentHost = new TestInsightHost();
        }

        [Fact]
        public void KnowsHowToParseSimpleSendEffect()
        {
            var url = new Uri("memory://file1");
            var contents = "on chat\n    send \"hi\"".SplitOnNewLines();
            var file = new SkriptFile(url)
            {
                RawContents =
                    contents
            };

            WorkspaceManager.Instance.HandleOpenedFile(file);
            file.HandleChange(new TextDocumentContentChangeEvent
            {
                Range = RangeExtensions.From(0, 7, 7),
                RangeLength = 1,
                Text = ":"
            });

            Assert.Equal(2, file.Nodes.Count);

            //Basic node match just to be sure
            for (var i = 0; i < file.Nodes.Count; i++)
                Assert.Equal(contents[i], file.Nodes[i].ToString());


            var results = new[] {"AsyncPlayerChatEvent", "EffMessage"};

            for (var i = 0; i < results.Length; i++)
                Assert.True(file.Nodes[i].IsMatchOfType(results[i]),
                    $"{i}: {file.Nodes[i].NodeContent} supposed to be {results[i]}");
        }

        [Fact]
        public void KnowsHowToHandleParentsAndChildren()
        {
            var url = new Uri("memory://file1");
            var contents = (
                "on chat\n" +
                "    if message contains \"h\"\n" +
                "        if message contains \"i\"\n" +
                "            send \"hi\""
            ).SplitOnNewLines();
            var file = new SkriptFile(url)
            {
                RawContents = contents
            };

            WorkspaceManager.Instance.HandleOpenedFile(file);
            file.HandleChange(new TextDocumentContentChangeEvent
            {
                Range = RangeExtensions.From(2, 31, 31),
                RangeLength = 1,
                Text = ":"
            });

            file.HandleChange(new TextDocumentContentChangeEvent
            {
                Range = RangeExtensions.From(1, 27, 27),
                RangeLength = 1,
                Text = ":"
            });

            file.HandleChange(new TextDocumentContentChangeEvent
            {
                Range = RangeExtensions.From(0, 7, 7),
                RangeLength = 1,
                Text = ":"
            });

            Assert.Equal(4, file.Nodes.Count);

            //Basic node match just to be sure
            for (var i = 0; i < file.Nodes.Count; i++)
                Assert.Equal(contents[i], file.Nodes[i].ToString());


            var results = new[] {"AsyncPlayerChatEvent", "", "", "EffMessage"};

            for (var i = 0; i < results.Length; i++)
                if (!string.IsNullOrEmpty(results[i]))
                    Assert.True(file.Nodes[i].IsMatchOfType(results[i]),
                        $"{i}: {file.Nodes[i].NodeContent} supposed to be {results[i]}");


            Assert.Equal(file.Nodes[0], file.Nodes[1].Parent);
            Assert.Equal(file.Nodes[1], file.Nodes[2].Parent);
            Assert.Equal(file.Nodes[2], file.Nodes[3].Parent);
        }
    }
}