using SkriptInsight.Model.Parser.Patterns.Impl;
using Xunit;

namespace SkriptInsight.Tests
{
    public class SkriptPatternOptionalsTests
    {
        [Fact]
        public void OptionalPatternAlwaysMatches()
        {
            var element = new OptionalPatternElement("b");
            Assert.True(element.Parse("a").IsSuccess);
            Assert.True(element.Parse("a").IsOptionallyMatched);
        }

        [Fact]
        public void OptionalPatternRenderIsCorrect()
        {
            var element = new OptionalPatternElement("abc");
            Assert.Equal("[abc]", element.RenderPattern());
        }
    }
}