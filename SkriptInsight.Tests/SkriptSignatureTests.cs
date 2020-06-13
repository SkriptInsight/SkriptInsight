using SkriptInsight.Core.Managers;
using SkriptInsight.Core.Parser.Signatures.ControlFlow;
using Xunit;

namespace SkriptInsight.Tests
{
    public class SkriptSignatureTests
    {
        public SkriptSignatureTests()
        {
            WorkspaceManager.CurrentHost = new TestInsightHost();
        }
        
        [Theory]
        [InlineData("if true", true)]
        [InlineData("if false", true)]
        [InlineData("if \"abc\" is alphanumeric", true)]
        [InlineData("if {homes::%uuid of player%::%arg-1%} is not set", true)]
        public void IfSignatureMatchesCorrectly(string code, bool valid)
        {
            Assert.Equal(valid, IfNodeSignature.TryParse(code) != null);
        }
    }
}