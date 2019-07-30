using System.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Model.Managers;
using SkriptInsight.Model.Parser;
using SkriptInsight.Model.Parser.Patterns;
using SkriptInsight.Model.Parser.Patterns.Impl;
using Xunit;
using static SkriptInsight.Model.Parser.Patterns.SyntaxValueAcceptanceConstraint;

namespace SkriptInsight.Tests
{
    public class SkriptPatternTypesTest
    {
        [Theory]
        [InlineData("type", None)]
        [InlineData("*type", LiteralsOnly)]
        [InlineData("~type", NoLiterals)]
        [InlineData("^type", VariablesOnly)]
        [InlineData("-type", NullWhenOmitted)]
        [InlineData("=type", AllowConditionalExpressions)]
        [InlineData("~=type", NoLiterals | AllowConditionalExpressions)]
        public void TypePatternConstraintsAreCorrect(string code, SyntaxValueAcceptanceConstraint expected)
        {
            var element = new TypePatternElement(code);
            Assert.Equal(expected, element.Constraint);
        }

        [Theory]
        [InlineData("\"test\"")]
        [InlineData("\"te\"\"st\"")]
        public void StringTypeParsesCorrectly(string input)
        {
            KnownTypesManager.Instance.LoadKnownTypes();
            var stringPattern = SkriptPattern.ParsePattern(ParseContext.FromCode("%string%"));

            var result = stringPattern.Parse(input);

            Assert.True(result.IsSuccess);
            Assert.Single(result.Context.Matches);
            Assert.Equal(new Range(new Position(0, 0), new Position(0, input.Length)),
                result.Context.Matches.First().Range);
        }

        [Theory]
        [InlineData("12")]
        [InlineData("1 2")]
        public void ChoicePatternGeneratesMatches(string input)
        {
            var pattern = SkriptPattern.ParsePattern("(1|one)[ ](2|two)");

            var result = pattern.Parse(input);
            
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Matches.Count);
            Assert.Equal("1", result.Matches.ElementAtOrDefault(0)?.Content);
            Assert.Equal("2", result.Matches.ElementAtOrDefault(1)?.Content);

        }

        [Theory]
        [InlineData("print", false)]
        [InlineData("println", false)]
        [InlineData("print \"Hello World\"")]
        [InlineData("println \"Hello World\"")]
        public void MixedStringTypeParsesCorrectly(string input, bool success = true)
        {
            KnownTypesManager.Instance.LoadKnownTypes();
            var stringPattern = SkriptPattern.ParsePattern(ParseContext.FromCode("print[ln] %string%"));

            var result = stringPattern.Parse(input);
            
            Assert.Equal(success, result.IsSuccess);
            if (success)
                Assert.Single(result.Context.Matches);
        }

        [Theory]
        [InlineData("print", false)]
        [InlineData("println", false)]
        [InlineData("print \"Hello World\"", false)]
        [InlineData("println \"Hello World\"", false)]
        [InlineData("print \"Hello World\" \"Hi\"")]
        [InlineData("println \"Hello World\" \"Howdy!\"")] 
        public void MixedDoubleStringTypeParsesCorrectly(string input, bool success = true)
        {
            KnownTypesManager.Instance.LoadKnownTypes();
            var stringPattern = SkriptPattern.ParsePattern(ParseContext.FromCode("print[ln] %string% %string%"));

            var result = stringPattern.Parse(input);
            
            Assert.Equal(success, result.IsSuccess);
            if (success)
                Assert.Equal(2, result.Context.Matches.Count);
        }
    }
}