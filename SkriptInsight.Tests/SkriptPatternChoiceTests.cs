using SkriptInsight.Core.Parser.Patterns.Impl;
using Xunit;

namespace SkriptInsight.Tests
{
    public class SkriptPatternChoiceTests
    {
        [Theory]
        [InlineData("one", 0)]
        [InlineData("1", 1)]
        public void ChoicePatternMatches(string toMatch, int expectedParseMark)
        {
            var element = new ChoicePatternElement("one|1¦1");

            var result = element.Parse(toMatch);
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedParseMark, result.ParseMark);
        }
    }
}