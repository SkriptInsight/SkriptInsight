using SkriptInsight.Core.Parser;
using SkriptInsight.Core.Parser.Patterns;
using Xunit;

namespace SkriptInsight.Tests
{
    public class SkriptPatternParseContextTests
    {
        [Theory]
        [InlineData("Test")]
        [InlineData("Test Abc")]
        [InlineData("Test %string% Abc")]
        public void ParseContextCanFindSingleBracket(string str)
        {
            var ctx = ParseContext.FromCode($"%{{{str}}}%");
            ctx.ReadNext(1); //Enter inside bracket

            Assert.Equal($"{{{str}}}", ctx.ReadUntilPosition(ctx.FindNextBracket('%', new[] {('{', '}')})));
        }

        [Theory]
        [InlineData("Test")]
        [InlineData("Test Abc")]
        [InlineData("Test \"\" Abc")]
        public void ParseContextCanFindQuote(string str)
        {
            var ctx = ParseContext.FromCode($"\"{str}\"");
            ctx.ReadNext(1); //Enter inside bracket

            Assert.Equal(str, ctx.ReadUntilPosition(ctx.FindNextBracket('"', true)));
        }

        [Theory]
        [InlineData("Test string")]
        [InlineData("Test[]string")]
        [InlineData("Test{}string")]
        [InlineData("Test<>string")]
        [InlineData("Test<()>string")]
        [InlineData("Test<([])>string")]
        public void ParseContextCanFindBracket(string str)
        {
            var ctx = ParseContext.FromCode($"[{str}]");
            ctx.ReadNext(1); //Enter inside bracket

            Assert.Equal(str, ctx.ReadUntilPosition(ctx.FindNextBracket('[', ']')));
        }

        [Theory]
        [InlineData("test,test", ',', 4)]
        [InlineData("te(,)st,test", ',', 7)]
        [InlineData("te(\",\")st,test", ',', 9)]
        [InlineData("te\",\"st,test", ',', 7)]
        [InlineData("te(\",\"\"\")st,test", ',', 11)]
        [InlineData("te(\"\"\",\"\"\")st,test", ',', 13)]
        public void ParseContextCanFindCharOutsideOfStuff(string str, char ch, int expectedPos)
        {
            var ctx = ParseContext.FromCode($"{str}");
            
            Assert.Equal(expectedPos, ctx.FindNextCharNotInsideNormalBracket(ch));
        }

        [Theory]
        [InlineData("(one|1¦1) (2¦two|2)", "1 two", 3)]
        [InlineData("(one|1¦1) (2¦two|2)", "one 2", 0)]
        [InlineData("(1¦show|2¦hide) holo[gra(m|phic display)][s]", "show hologram", 1)]
        [InlineData("(1¦show|2¦hide) holo[gra(m|phic display)][s]", "show holo", 1)]
        [InlineData("(1¦show|2¦hide) holo[gra(m|phic display)][s]", "hide hologram", 2)]
        [InlineData("(1¦show|2¦hide) holo[gra(m|phic display)][s]", "hide holos", 2)]
        public void ParseContextParseMarksMatch(string pattern, string code, int expectedParseMark)
        {
            var ctx = ParseContext.FromCode(code);
            var skriptPattern = SkriptPattern.ParsePattern(ParseContext.FromCode(pattern));

            var result = skriptPattern.Parse(ctx);
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedParseMark, result.ParseMark);
        }
    }
}