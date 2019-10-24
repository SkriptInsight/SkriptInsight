using SkriptInsight.Core.Parser;
using Xunit;

namespace SkriptInsight.Tests
{
    public class SkriptNounParserTests
    {
        [Theory]
        [InlineData("tree¦s @a", "tree", "trees", "a")]
        [InlineData("arrow¦s @an", "arrow", "arrows", "an")]
        [InlineData("zombie pig¦man¦men", "zombie pigman", "zombie pigmen", "a")]
        [InlineData("bottle¦ of enchanting¦s of enchanting", "bottle of enchanting", "bottles of enchanting", "a")]
        [InlineData("living entit¦y¦ies", "living entity", "living entities", "a")]
        [InlineData("spectral arrow", "spectral arrow", "", "a")]
        [InlineData("spectral arrow @a", "spectral arrow", "", "a")]
        public void NounParserCanParseCorrectly(string noun, string singular, string plural, string gender)
        {
            var (gResult, sResult, pResult) = SkriptNounParser.ExtractInformationFromNoun(noun);
            
            Assert.Equal(gender, gResult);
            Assert.Equal(singular, sResult);
            Assert.Equal(plural, pResult);
        }
        [Theory]
        [InlineData("tree¦s @a", "(4¦trees|2¦[(1¦a )]tree)")]
        [InlineData("arrow¦s @an", "(4¦arrows|2¦[(1¦an )]arrow)")]
        [InlineData("zombie pig¦man¦men", "(4¦zombie pigmen|2¦[(1¦a )]zombie pigman)")]
        [InlineData("bottle¦ of enchanting¦s of enchanting", "(4¦bottles of enchanting|2¦[(1¦a )]bottle of enchanting)")]
        [InlineData("living entit¦y¦ies", "(4¦living entities|2¦[(1¦a )]living entity)")]
        [InlineData("spectral arrow", "(2¦[(1¦a )]spectral arrow)")]
        [InlineData("spectral arrow @a", "(2¦[(1¦a )]spectral arrow)")]
        public void NounParserCanConvertCorrectly(string noun, string expectedPattern)
        {
            var result = SkriptNounParser.ConvertNounToPattern(noun);

            Assert.Equal(expectedPattern, result.RenderPattern());
        }
    }
}