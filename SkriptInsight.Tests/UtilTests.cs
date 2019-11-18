using SkriptInsight.Core.Files.Nodes.Impl;
using SkriptInsight.Core.Parser.Functions;
using SkriptInsight.Core.Utils;
using Xunit;

namespace SkriptInsight.Tests
{
    public class UtilTests
    {

        [Fact]
        public void ConstructionUtilsCanConstructObject()
        {
            var result = typeof(FunctionSignatureFileNode).NewInstance(new FunctionSignature());

            Assert.NotNull(result);
            Assert.IsType<FunctionSignatureFileNode>(result);
        }

        [Fact]
        public void CanGetDelegateForFunctionSignature()
        {
            var del = SignatureParserHelper.GetTryParseDelegateForType(typeof(FunctionSignature));
            
            Assert.NotNull(del);
        }
        
    }
}