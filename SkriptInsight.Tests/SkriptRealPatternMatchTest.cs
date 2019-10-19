using System.Linq;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Parser;
using SkriptInsight.Core.Parser.Expressions;
using SkriptInsight.Core.Parser.Patterns;
using SkriptInsight.Core.Parser.Types.Impl;
using SkriptInsight.Core.Types;
using Xunit;

namespace SkriptInsight.Tests
{
    public class SkriptRealPatternMatchTest
    {
        [Fact]
        public void MatchesCorrectly()
        {
            var color = SkriptPattern.ParsePattern("colored %string% with %color%");

            var result = color.Parse("colored \"test\" with red");

            var resultColors = result.Matches.OfType<ExpressionParseMatch>()
                .Last().Expression.GetValues<ChatColor>()
                .ToList();


            Assert.True(result.IsSuccess);
        }


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