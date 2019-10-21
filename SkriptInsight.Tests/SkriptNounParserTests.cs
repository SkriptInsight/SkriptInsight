using SkriptInsight.Core.Parser;
using Xunit;

namespace SkriptInsight.Tests
{
    public class SkriptNounParserTests
    {
        [Theory]
        [InlineData("tree¦s @a", "tree", "trees", "a")]
        [InlineData("arrow¦s @an", "arrow", "arrows", "an")]
        [InlineData("zombie pig¦man¦men", "zombie pigman", "zombie pigmen", "")]
        [InlineData("bottle¦ of enchanting¦s of enchanting", "bottle of enchanting", "bottles of enchanting", "")]
        [InlineData("living entit¦y¦ies", "living entity", "living entities", "")]
        [InlineData("spectral arrow", "spectral arrow", "", "")]
        [InlineData("spectral arrow @a", "spectral arrow", "", "a")]
        public void NounParserCanParseCorrectly(string noun, string singular, string plural, string gender)
        {
            var (gResult, sResult, pResult) = SkriptNounParser.ParseNoun(noun);
            
            Assert.Equal(gender, gResult);
            Assert.Equal(singular, sResult);
            Assert.Equal(plural, pResult);
        }
        [Theory]
        [InlineData("tree¦s @a", "([a ]tree|trees)")]
        [InlineData("arrow¦s @an", "([an ]arrow|arrows)")]
        [InlineData("zombie pig¦man¦men", "(zombie pigman|zombie pigmen)")]
        [InlineData("bottle¦ of enchanting¦s of enchanting", "(bottle of enchanting|bottles of enchanting)")]
        [InlineData("living entit¦y¦ies", "(living entity|living entities)")]
        [InlineData("spectral arrow", "spectral arrow")]
        [InlineData("spectral arrow @a", "[a ]spectral arrow")]
        public void NounParserCanConvertCorrectly(string noun, string expectedPattern)
        {
            var result = SkriptNounParser.ConvertNounToPattern(noun);

            Assert.Equal(expectedPattern, result.RenderPattern());
        }
    }
}