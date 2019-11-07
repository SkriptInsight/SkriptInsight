using SkriptInsight.Core.Parser.Functions;
using SkriptInsight.Core.Utils;
using Xunit;

namespace SkriptInsight.Tests
{
    public class SignatureDelegatesTests
    {

        [Fact]
        public void CanGetDelegateForFunctionSignature()
        {
            var del = SignatureParserHelper.GetTryParseDelegateForType(typeof(FunctionSignature));
            
            Assert.NotNull(del);
        }
        
    }
}