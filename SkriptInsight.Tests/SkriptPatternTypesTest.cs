using System.Diagnostics;
using System.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Extensions;
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
        public SkriptPatternTypesTest()
        {
            //Just to preload the workspace variable
            var workspace = WorkspaceManager.CurrentWorkspace;
        }

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
            WorkspaceManager.Instance.KnownTypesManager.LoadKnownTypes();
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
            Assert.Equal(input.Contains(" ") ? 3 : 2, result.Matches.Count);
            Assert.Equal("1", result.Matches.ElementAtOrDefault(0)?.RawContent);
            Assert.Equal("2", result.Matches.ElementAtOrDefault(input.Contains(" ") ? 2 : 1)?.RawContent);
        }

        [Theory]
        [InlineData("print", false)]
        [InlineData("println", false)]
        [InlineData("print \"Hello World\"")]
        [InlineData("println \"Hello World\"")]
        public void MixedStringTypeParsesCorrectly(string input, bool success = true)
        {
            WorkspaceManager.Instance.KnownTypesManager.LoadKnownTypes();
            var stringPattern = SkriptPattern.ParsePattern(ParseContext.FromCode("print[ln] %string%"));

            var result = stringPattern.Parse(input);

            Assert.Equal(success, result.IsSuccess);
            if (success)
                Assert.Single(result.Context.Matches.OfType<ExpressionParseMatch>());
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
            WorkspaceManager.Instance.KnownTypesManager.LoadKnownTypes();
            var stringPattern = SkriptPattern.ParsePattern(ParseContext.FromCode("print[ln] %string% %string%"));

            var result = stringPattern.Parse(input);

            Assert.Equal(success, result.IsSuccess);
            if (success)
                Assert.Equal(input.Contains("println") ? 3 : 2, result.Context.Matches.Count);
        }

        [Theory]
        [InlineData("\"test\" and \"test\"")]
        [InlineData("(\"test\") and \"test\"")]
        public void MixedParenthesesTypeParsesCorrectly(string input)
        {
            WorkspaceManager.Instance.KnownTypesManager.LoadKnownTypes();
            var stringPattern = SkriptPattern.ParsePattern("print %strings%");

            var result = stringPattern.Parse("print " + input);

            Assert.True(result.IsSuccess, "result.IsSuccess");
            Assert.True(result.Context.HasFinishedLine, "result.Context.HasFinishedLine");
        }

        [Fact]
        public void MixedParenthesesWithMultipleValuesParsesCorrectly()
        {
            const string input = "(\"true\" and \"reee\") and \"false\"";
            WorkspaceManager.Instance.KnownTypesManager.LoadKnownTypes();
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
            WorkspaceManager.Instance.KnownTypesManager.LoadKnownTypes();
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
            WorkspaceManager.Instance.KnownTypesManager.LoadKnownTypes();
            var stringPattern = SkriptPattern.ParsePattern(ParseContext.FromCode("print %string%"));

            var result = stringPattern.Parse("print " + input);

            Assert.True(result.IsSuccess, "result.IsSuccess");
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
        [InlineData("strings", "\"one\", \"two\", \"three\" and \"four\"")]
        [InlineData("boolean", "true")]
        [InlineData("boolean", "false")]
        [InlineData("boolean", "on")]
        [InlineData("boolean", "off")]
        [InlineData("boolean", "yes")]
        [InlineData("boolean", "no")]
        [InlineData("booleans", "true, false")]
        [InlineData("booleans", "false and true")]
        [InlineData("booleans", "false, true and no")]
        [InlineData("number", "1")]
        [InlineData("number", "-2")]
        [InlineData("number", "-2.3")]
        [InlineData("numbers", "-2.3, 1, -2, 5")]
        [InlineData("numbers", "-2.1234567")]
        [InlineData("color", "red")]
        [InlineData("colors", "blue and red")]
        [InlineData("click type", "creative action")]
        [InlineData("click types", "middle mouse button or left mouse button")]
        [InlineData("difficulty", "medium")]
        [InlineData("difficulties", "medium or normal")]
        [InlineData("enchantment", "unbreaking")]
        [InlineData("enchantments", "unbreaking and infinity")]
        [InlineData("enchantment type", "efficiency 5")]
        [InlineData("enchantment type", "efficiency")]
        [InlineData("entitydata", "arrow")]
        [InlineData("entitydata", "an arrow")]
        [InlineData("entitydata", "blaze")]
        [InlineData("entitydatas", "blaze and an arrow")]
        [InlineData("entitytype", "2 arrows")]
        [InlineData("entitytype", "1 arrow")]
        [InlineData("entitytype", "a blaze")]
        [InlineData("entitytypes", "2 blazes")]
        [InlineData("entity types", "a blaze and 2 arrows")]
        [InlineData("weathers", "raining and sunny")]
        [InlineData("weather conditions", "raining or sunny or thunder")]
        [InlineData("weather types", "raining or sunny and thunder")]
        [InlineData("gamemode", "survival")]
        [InlineData("gamemode", "spectator")]
        [InlineData("game mode", "creative")]
        [InlineData("game modes", "adventure or creative or survival or spectator")]
        [InlineData("experience", "1 xp")]
        [InlineData("experience", "3 exp")]
        [InlineData("experience", "1 experience")]
        [InlineData("experience", "1 experience point")]
        [InlineData("experience", "3 experience")]
        [InlineData("experience", "3 experience points")]
        public void TypesCanBeRepresentedAsStrings(string type, string value)
        {
            //Parse normal type from name
            {
                var pattern = SkriptPattern.ParsePattern($"%{type}%");
                var result = pattern.Parse(value);

                Assert.True(result.IsSuccess, "result.IsSuccess");
                Assert.Single(result.Matches);
                Assert.True(result.Context.HasFinishedLine, $"result.Context.HasFinishedLine =>{result.Context.PeekUntilEnd()}");

                var match = result.Matches.First();
                Assert.IsType<ExpressionParseMatch>(match);
                var exprMatch = match as ExpressionParseMatch;
                Assert.Equal(value, exprMatch?.Expression.AsString() ?? "--NULL--");
            }

            //Parse type from generic object type
            {
                var pattern = SkriptPattern.ParsePattern($"%object{(type.IsPlural() ? "s" : "")}%");
                var result = pattern.Parse(value);

                Assert.True(result.IsSuccess, "[object]result.IsSuccess");
                Assert.Single(result.Matches);
                Assert.True(result.Context.HasFinishedLine, "[object]result.Context.HasFinishedLine");

                var match = result.Matches.First();
                Assert.IsType<ExpressionParseMatch>(match);
                var exprMatch = match as ExpressionParseMatch;
                Assert.Equal(value, exprMatch?.Expression.AsString() ?? "--NULL--");
            }
        }

        [Theory]
        [InlineData("\"abc\" is alphanumeric")]
        public void ConditionalTypeMatchingWorks(string code)
        {
            var pattern = new SkriptPattern
            {
                Children =
                {
                    new TypePatternElement
                    {
                        Constraint = AllowConditionalExpressions,
                        Type = "boolean"
                    }
                }
            };
            
            Assert.True(pattern.Parse(code).IsSuccess, "pattern.Parse(code).IsSuccess");
        }

        [Fact]
        public void AllColorsCanBeParsedCorrectly()
        {
            var colors = new[]
            {
                "black", "dark grey", "dark gray", "grey", "light grey", "gray", "light gray", "silver", "white",
                "blue", "dark blue", "brown", "light blue", "indigo", "cyan", "aqua", "dark cyan", "dark aqua",
                "dark turquoise", "dark turquois", "light cyan", "light aqua", "turquoise", "turquois", "light blue",
                "green", "dark green", "light green", "lime", "lime green", "yellow", "light yellow", "orange", "gold",
                "dark yellow", "red", "dark red", "pink", "light red", "purple", "dark purple", "magenta",
                "light purple"
            };
            var pattern = SkriptPattern.ParsePattern("%color%");

            foreach (var color in colors)
            {
                var ctx = ParseContext.FromCode(color);
                var result = pattern.Parse(ctx);

                Assert.True(result.IsSuccess, $"result.IsSuccess -> {color}");
                Assert.False(result.IsOptionallyMatched);
            }
        }

        [Theory]
        [InlineData("test")]
        [InlineData("aa:aa")]
        [InlineData("a:a::a:a")]
        public void InternalFunctionParamNameTypeCanMatchCorrectly(string input)
        {
            var pattern = SkriptPattern.ParsePattern("%si_func_param_name%:");
            var result = pattern.Parse($"{input}: aaa, test2: bbbb");

            Assert.True(result.IsSuccess);
            Assert.Single(result.Matches);

            Assert.IsType<ExpressionParseMatch>(result.Matches[0]);
            var resultMatch = result.Matches[0] as ExpressionParseMatch;
            Debug.Assert(resultMatch != null, nameof(resultMatch) + " != null");

            Assert.IsType<Expression<string>>(resultMatch.Expression);
            var strExpr = resultMatch.Expression as Expression<string>;
            Debug.Assert(strExpr != null, nameof(strExpr) + " != null");

            Assert.Equal(input, strExpr.GenericValue);
        }

        [Fact]
        public void TypeParserCanMatchNonLiteralExpressions()
        {
            WorkspaceManager.Instance.KnownTypesManager.LoadKnownTypes();
            var pattern = SkriptPattern.ParsePattern("(message|send [message[s]]) %strings% [to %commandsenders%]");

            var result = pattern.Parse("send \"hi\" to all players");

            Assert.True(result.IsSuccess);
        }


        [Fact]
        public void AllClickTypesCanBeParsedCorrectly()
        {
            var clicks = new[]
            {
                "left mouse button", "left mouse", "LMB", "left mouse button with shift", "left mouse with shift",
                "Shift+RMB", "right mouse button", "right mouse", "RMB", "right mouse button with shift",
                "right mouse with shift", "Shift+RMB", "window border using right mouse button",
                "window border using right mouse", "border using LMB", "window border using left mouse button",
                "window border using right mouse", "border using RMB", "middle mouse button", "middle mouse", "MMB",
                "number key", "0-9", "double click using mouse", "double click", "drop key", "drop item", "Q",
                "drop key with control", "drop stack", "Ctrl+Q", "creative action", "unknown", "unsupported", "custom"
            };

            var pattern = SkriptPattern.ParsePattern("%click type%");

            foreach (var click in clicks)
            {
                var ctx = ParseContext.FromCode(click);
                var result = pattern.Parse(ctx);

                Assert.True(result.IsSuccess, $"result.IsSuccess -> {click}");
                Assert.False(result.IsOptionallyMatched);
            }
        }
    }
}