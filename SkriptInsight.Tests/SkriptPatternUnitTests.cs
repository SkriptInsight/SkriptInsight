using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SkriptInsight.Model.Parser;
using SkriptInsight.Model.Parser.Expressions;
using SkriptInsight.Model.Parser.Patterns;
using SkriptInsight.Model.Parser.Patterns.Impl;
using Xunit;

namespace SkriptInsight.Tests
{
    public class SkriptPatternUnitTests
    {
        [Fact]
        public void FullPatternParsesChoiceCorrectly()
        {
            var pattern = SkriptPattern.ParsePattern(ParseContext.FromCode("(a|b|c)"));

            Assert.Single(pattern.Children);
            Assert.IsType<ChoicePatternElement>(pattern.Children.First());
        }

        [Fact]
        public void FullPatternParsesMultipleChoiceCorrectly()
        {
            var pattern = SkriptPattern.ParsePattern(ParseContext.FromCode("(a|b)(c|d)"));

            Assert.Equal(2, pattern.Children.Count);
            Assert.IsType<ChoicePatternElement>(pattern.Children.First());
            Assert.IsType<ChoicePatternElement>(pattern.Children.Skip(1).First());
        }

        [Fact]
        public void FullPatternParsesMultipleInnerChoiceCorrectly()
        {
            var pattern = SkriptPattern.ParsePattern(ParseContext.FromCode("((a|e)|(b|f))((c|g)|(d|h))"));

            Assert.Equal(2, pattern.Children.Count);
            Assert.IsType<ChoicePatternElement>(pattern.Children.First());
            Assert.IsType<ChoicePatternElement>(pattern.Children.Skip(1).First());
            Assert.Equal("((a|e)|(b|f))((c|g)|(d|h))", pattern.RenderPattern());
        }

        [Theory]
        [InlineData("broadcast %texts% [(to|in) %worlds%]")]
        public void NormalSkriptPatternParsesCorrectly(string skPattern)
        {
            var pattern = SkriptPattern.ParsePattern(ParseContext.FromCode(skPattern));
            Assert.Equal(skPattern, pattern.RenderPattern());
        }

        [Theory]
        [InlineData("(message|send[ message[s]]) %string%[ to %commandsenders%]", "message \"hi\"")]
        [InlineData("(message|send[ message[s]]) %string%[ to %commandsenders%]", "send \"hi\"")]
        [InlineData("(message|send[ message[s]]) %string%[ to %commandsenders%]", "send message \"hi\"")]
        public void NormalSkriptPatternMatchesCorrectly(string pattern, string input)
        {
            var skPattern = SkriptPattern.ParsePattern(pattern);

            var result = skPattern.Parse(input);

            Assert.True(result.IsSuccess);
            Assert.True(result.Context.HasFinishedLine);
        }

        [Theory]
        [InlineData("\"test\"", true, new[] {"test"})]
        [InlineData("\"test1\" abc \"test2\"", false)]
        [InlineData("\"test1\" and \"test2\"", true, new[] {"test1", "test2"})]
        [InlineData("\"test1\", \"test2\", \"test3\", \"test4\"", true, new[] {"test1", "test2", "test3", "test4"})]
        public void MultiStringsPatternMatchesCorrectly(string input, bool shouldFinish = true,
            string[] inputtedValues = null)
        {
            var pattern = SkriptPattern.ParsePattern("%strings%");

            var result = pattern.Parse(input);
            //TODO: Implement match structure to see what elements matched for each child node
            Assert.Single(result.Matches);

            var parseMatch = result.Matches.First();
            Assert.IsType<ExpressionParseMatch>(parseMatch);

            var exprParseMatch = parseMatch as ExpressionParseMatch;
            Debug.Assert(exprParseMatch != null, nameof(exprParseMatch) + " != null");
            Assert.IsType<Expression<List<IExpression>>>(exprParseMatch.Expression);

            var expr = exprParseMatch.Expression as Expression<List<IExpression>>;

            Debug.Assert(expr != null, nameof(expr) + " != null");
            expr.GenericValue.ForEach(e => Assert.IsType<Expression<string>>(e));

            var expressions = expr.GenericValue.Cast<Expression<string>>().ToArray();
            if (inputtedValues != null)
            {
                for (var i = 0; i < inputtedValues.Length; i++)
                {
                    Assert.Equal(expressions[i]?.GenericValue, inputtedValues[i]);
                }
            }

            Assert.Equal(shouldFinish, result.Context.HasFinishedLine);
        }

        [Theory]
        [InlineData("testing life")]
        [InlineData("testing this thing i call life")]
        public void DanglingSpacesPatternMatchesCorrectly(string input)
        {
            var skPattern = SkriptPattern.ParsePattern("testing [this [fancy] thing] [i call] life");

            var result = skPattern.Parse(input);

            Assert.True(result.IsSuccess);
            Assert.True(result.Context.HasFinishedLine);
        }
    }
}