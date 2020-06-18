using System;
using System.Diagnostics;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Files;
using SkriptInsight.Core.Managers;
using Xunit;

namespace SkriptInsight.Tests
{
    public class SkriptFileStructureTests
    {
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
                RawContents = contents
            };

            WorkspaceManager.Instance.HandleOpenedFile(file);
            file.PrepareNodes();

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


            var results = new[] {"org.bukkit.event.player.AsyncPlayerChatEvent", "ch.njol.skript.effects.EffMessage"};

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
            file.PrepareNodes();

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


            var results = new[]
                {"org.bukkit.event.player.AsyncPlayerChatEvent", "", "", "ch.njol.skript.effects.EffMessage"};

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