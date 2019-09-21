using System.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Managers;
using SkriptInsight.Core.Parser;
using SkriptInsight.Core.Parser.Expressions;
using SkriptInsight.Core.Parser.Expressions.Variables;
using SkriptInsight.Core.Parser.Patterns;
using SkriptInsight.Core.Parser.Patterns.Impl;
using Xunit;
using static SkriptInsight.Core.Parser.Patterns.SyntaxValueAcceptanceConstraint;

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
            Assert.Equal("1", result.Matches.ElementAtOrDefault(0)?.RawContent);
            Assert.Equal("2", result.Matches.ElementAtOrDefault(1)?.RawContent);
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

        [Theory]
        [InlineData("\"test\" and \"test\"")]
        [InlineData("(\"test\") and \"test\"")]
        public void MixedParenthesesTypeParsesCorrectly(string input)
        {
            
            KnownTypesManager.Instance.LoadKnownTypes();
            var stringPattern = SkriptPattern.ParsePattern("print %strings%");

            var result = stringPattern.Parse("print " + input);

            Assert.True(result.IsSuccess, "result.IsSuccess");
            Assert.True(result.Context.HasFinishedLine, "result.Context.HasFinishedLine");
        }

        [Fact]
        public void MixedParenthesesWithMultipleValuesParsesCorrectly()
        {
            const string input = "(\"true\" and \"reee\") and \"false\"";
            KnownTypesManager.Instance.LoadKnownTypes();
            var stringPattern = SkriptPattern.ParsePattern("print %strings%");

            var result = stringPattern.Parse("print " + input);

            Assert.True(result.IsSuccess, "result.IsSuccess");
            Assert.True(result.Context.HasFinishedLine, "result.Context.HasFinishedLine");
        }
        
        [Theory]
        [InlineData("(\"test\")")]
        [InlineData("((\"test\"))")]
        [InlineData("(((\"test\")))")]
        public void ParenthesesTypeParsesCorrectly(string input)
        {
            KnownTypesManager.Instance.LoadKnownTypes();
            var stringPattern = SkriptPattern.ParsePattern(ParseContext.FromCode("print %string%"));

            var result = stringPattern.Parse("print " + input);

            Assert.True(result.IsSuccess, "result.IsSuccess");
            Assert.Single(result.Context.Matches);

            var match = result.Context.Matches.First();
            Assert.IsType<ExpressionParseMatch>(match);
            
            var expr = ((ExpressionParseMatch) match).Expression;
            Assert.IsType<ParenthesesExpression>(expr);
            
            var parenthesesExpression = expr as ParenthesesExpression;
            Assert.NotNull(parenthesesExpression);
            Assert.Equal(input, parenthesesExpression.AsString());
            
        }
        
        [Theory]
        [InlineData("{_test}")]
        [InlineData("{_test.%test%.a}")]
        [InlineData("{_test.2}")]
        [InlineData("{_test::2}")]
        [InlineData("{_test::2::3}")]
        public void MixedVariableTypeParsesCorrectly(string input)
        {
            KnownTypesManager.Instance.LoadKnownTypes();
            var stringPattern = SkriptPattern.ParsePattern(ParseContext.FromCode("print %string%"));

            var result = stringPattern.Parse("print " + input);

            Assert.True(result.IsSuccess);
            Assert.Single(result.Context.Matches);

            var match = result.Context.Matches.First();
            Assert.IsType<ExpressionParseMatch>(match);
            
            var expr = ((ExpressionParseMatch) match).Expression;
            Assert.IsType<Expression<SkriptVariable>>(expr);
            
            var variable = expr.Value as SkriptVariable;
            Assert.NotNull(variable);
            Assert.Equal(input, variable.ToString());
        }
        
        
        [Theory]
        [InlineData("string", "\"test\"")]
        [InlineData("string", "\"te\"\"st\"")]
        [InlineData("strings", "\"test\"")]
        [InlineData("strings", "\"one\" and \"two\"")]
        [InlineData("strings", "\"one\", \"two\" and \"three\"")]
        [InlineData("boolean", "true")]
        [InlineData("boolean", "false")]
        [InlineData("boolean", "on")]
        [InlineData("boolean", "off")]
        [InlineData("boolean", "yes")]
        [InlineData("boolean", "no")]
        [InlineData("booleans", "true, false")]
        [InlineData("booleans", "false and true")]
        [InlineData("booleans", "false, true and no")]
        public void TypesCanBeRepresentedAsStrings(string type, string value)
        {
            var pattern = SkriptPattern.ParsePattern($"%{type}%");
            var result = pattern.Parse(value);
            
            Assert.True(result.IsSuccess);
            Assert.Single(result.Matches);
            
            var match = result.Matches.First();
            Assert.IsType<ExpressionParseMatch>(match);
            var exprMatch = match as ExpressionParseMatch;

            Assert.Equal(value, exprMatch?.Expression.AsString() ?? "--NULL--");
        }
    }
}