using SkriptInsight.Core.Parser.Patterns.Impl;
using Xunit;

namespace SkriptInsight.Tests
{
    public class SkriptPatternLiteralTests
    {
        [Fact]
        public void LiteralPatternMatches()
        {
            var element = new LiteralPatternElement("abc");
            Assert.True(element.Parse("abc").IsSuccess);
        }

        [Fact]
        public void LiteralPatternRenderIsCorrect()
        {
            var element = new LiteralPatternElement("abc");
            Assert.Equal("abc", element.RenderPattern());
        }

        [Fact]
        public void LiteralPatternDoesNotMatchWrong()
        {
            var element = new LiteralPatternElement("abc");
            Assert.False(element.Parse("def").IsSuccess);
        }
    }
}