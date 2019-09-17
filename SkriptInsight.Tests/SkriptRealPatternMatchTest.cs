using SkriptInsight.Core.Parser.Patterns;
using Xunit;

namespace SkriptInsight.Tests
{
    public class SkriptRealPatternMatchTest
    {
        [Theory]
        [InlineData("send \"message\"")]
        [InlineData("send message \"message\"")]
        [InlineData("send messages \"message\", \"reeee\"")]
        [InlineData("message \"message\"")]
        public void MatchesMessagePlayer(string code)
        {
            var pattern = SkriptPattern.ParsePattern("(message|send [message[s]]) %strings% [to %commandsenders%]");

            var result = pattern.Parse(code);

            Assert.True(result.IsSuccess);
        }
    }
}