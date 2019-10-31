using System.Linq;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Parser;
using SkriptInsight.Core.Parser.Expressions;
using SkriptInsight.Core.Parser.Functions;
using SkriptInsight.Core.Parser.Types.Impl.Internal;
using Xunit;

namespace SkriptInsight.Tests
{
    public class SkriptFunctionParserTests
    {
        [Theory]
        [InlineData("test", "string")]
        [InlineData("aaaa", "number")]
        [InlineData("bla", "integer")]
        [InlineData("aa:aa", "number")]
        public void FunctionParameterParserWorks(string expectedName, string expectedType)
        {
            var param = new SkriptFunctionParameter();
            var input = $"{expectedName}: {expectedType}";
            var result = param.TryParseValue(input);

            Assert.NotNull(result);
            var expr = result.GetExplicitValue<FunctionParameter>(0)?.GenericValue;

            Assert.NotNull(expr);

            Assert.Equal(expectedName, expr.Name);
            Assert.Equal(expectedType, expr.Type.TypeName);
            Assert.Equal(input, expr.ToString());
        }

        [Theory]
        [InlineData("test", "", "test: string", "testing2: number")]
        [InlineData("test", "string", "test: string", "testing2: number")]
        public void FunctionSignatureParserWorks(string name, string returnType = "", params string[] input)
        {
            var result = $"function {name}({string.Join(", ", input)}){(returnType != "" ? $" :: {returnType}" : "")}";

            var ctx = ParseContext.FromCode(result);
            var signature = FunctionSignature.TryParse(ctx);
            
            Assert.NotNull(signature);
            
            Assert.Equal(result, signature.ToString());
            
            Assert.Equal(name, signature.Name);
            
            if (!returnType.IsEmpty()) Assert.Equal(returnType, signature.ReturnType.TypeName);
            
            Assert.Equal(input, signature.Parameters.Select(p => p.ToString()));
            
            Assert.True(ctx.HasFinishedLine, "ctx.HasFinishedLine");
        }
    }
}