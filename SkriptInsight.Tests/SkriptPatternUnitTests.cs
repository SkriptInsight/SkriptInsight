using System;
using System.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Model.Managers;
using SkriptInsight.Model.Parser;
using SkriptInsight.Model.Parser.Patterns;
using SkriptInsight.Model.Parser.Patterns.Impl;
using Xunit;
using static SkriptInsight.Model.Parser.Patterns.SyntaxValueAcceptanceConstraint;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

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
        [InlineData("\"test\"")]
        [InlineData("\"test1\" abc \"test2\"", false)]
        [InlineData("\"test1\" and \"test2\"")]
        [InlineData("\"test1\", \"test2\", \"test3\", \"test4\", \"test5\"")]
        public void MultiStringsPatternMatchesCorrectly(string input, bool shouldFinish = true)
        {
            var pattern = SkriptPattern.ParsePattern("%strings%");

            var result = pattern.Parse(input);
            //TODO: Implement match structure to see what elements matched for each child node
            Assert.Single(result.Matches);
            Assert.Equal(shouldFinish, result.Context.HasFinishedLine);
        }
    }
}