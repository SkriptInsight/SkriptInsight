using System;
using System.Linq;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Files;
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

            var file = new SkriptFile(MemoryFile) {RawContents = fileContent.SplitOnNewLinesArray()};

            for (var i = 0; i < count; i++)
            {
                Assert.Equal(selector, file.ParseContext.ReadNext(selector.Length));
            }
            Assert.True(file.ParseContext.HasReachedEndOfFile);
        }
    }
}