using System;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Files;
using SkriptInsight.Core.Managers;
using SkriptInsight.Core.Parser;
using SkriptInsight.Core.Parser.Expressions;
using SkriptInsight.Core.Parser.Patterns;
using SkriptInsight.Core.Parser.Types.Impl;
using SkriptInsight.Core.Types;
using Xunit;
using Xunit.Abstractions;

namespace SkriptInsight.Tests
{
    public class SkriptRealPatternMatchTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public SkriptRealPatternMatchTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            WorkspaceManager.CurrentHost = new TestInsightHost();
        }

        [Fact]
        public void EmptyUnitTest()
        {
            // Debugger.Break();
            var code = "{_test}.hello().world().bruh()";
            
            var file = new SkriptFile(new Uri("memory://file"))
            {
                RawContents = ("on chat:\n" +
                               "    " + code).SplitOnNewLines()
            };
            WorkspaceManager.Instance.HandleOpenedFile(file);
            file.PrepareNodes();

            var node = file.Nodes[1];
            _testOutputHelper.WriteLine(node.ToJson());
        }
        
        [Fact]
        public void RandomWeirdPatternMatchesCorrectly()
        {
            var color = SkriptPattern.ParsePattern("colored %string% with %color%");

            var result = color.Parse("colored \"test\" with red");

            var resultColors = result.Matches.OfType<ExpressionParseMatch>()
                .Last().Expression.GetValues<ChatColor>()
                .ToList();

            Assert.True(result.IsSuccess);
        }


        [Theory]
        [InlineData("send \"message\" to player's inventory", 4)]
        [InlineData("send messages \"message\", \"reeee\" to player", 6)]
        public void MatchesMessagePlayer(string code, int expectedMatches)
        {
            var file = new SkriptFile(new Uri("memory://file"))
            {
                RawContents = ("on chat:\n" +
                               "    " + code).SplitOnNewLines()
            };

            WorkspaceManager.Instance.HandleOpenedFile(file);
            file.PrepareNodes();
            var effMessageNode = file.Nodes[1];
            
            Assert.NotNull(effMessageNode);
            Assert.NotNull(effMessageNode.MatchedSyntax);
            Assert.True(effMessageNode.MatchedSyntax.Result.IsSuccess);
            Assert.Equal(expectedMatches, effMessageNode.MatchedSyntax.Result.Matches.Count);
        }
    }
}