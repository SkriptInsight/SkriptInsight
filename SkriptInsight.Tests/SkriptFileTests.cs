using System;
using System.Collections.Generic;
using System.Linq;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Files;
using SkriptInsight.Core.Files.Nodes;
using Xunit;

namespace SkriptInsight.Tests
{
    public class SkriptFileTests
    {
        private static readonly Uri MemoryFile = new Uri("insight://memory-file.sk");

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(4)]
        [InlineData(8)]
        [InlineData(20)]
        public void FileKnowsHowToSwitchLinesOnTheContext(int count)
        {
            const string selector = "abcd";
            var fileContent = string.Join("\n", Enumerable.Range(0, count).Select(_ => selector));

            var file = new SkriptFile(MemoryFile) {RawContents = fileContent.SplitOnNewLines()};

            for (var i = 0; i < count; i++)
            {
                Assert.Equal(selector, file.ParseContext.ReadNext(selector.Length));
            }

            Assert.True(file.ParseContext.HasReachedEndOfFile);
        }


        public static IEnumerable<object[]> NodeIndentationWorksFineData =>
            new List<object[]>
            {
                new object[] {"test", new NodeIndentation[0]},
                new object[] {" test", new[] {new NodeIndentation(IndentType.Space, 1)}},
                new object[] {"  test", new[] {new NodeIndentation(IndentType.Space, 2)}},
                new object[]
                {
                    " \t test", new[]
                    {
                        new NodeIndentation(IndentType.Space, 2),
                        new NodeIndentation(IndentType.Tab, 1)
                    }
                },
            };

        [Theory]
        [MemberData(nameof(NodeIndentationWorksFineData))]
        public void NodeIndentationWorksFine(string text, NodeIndentation[] indentations)
        {
            var nodeIndentations = text.GetNodeIndentations();

            Assert.Equal(nodeIndentations, indentations.AsEnumerable().OrderBy(c => c.Type));
        }
    }
}