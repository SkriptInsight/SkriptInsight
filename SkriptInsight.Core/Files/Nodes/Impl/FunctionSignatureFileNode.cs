using SkriptInsight.Core.Parser.Functions;
using SkriptInsight.Core.SyntaxInfo;

namespace SkriptInsight.Core.Files.Nodes.Impl
{
    [SectionNode]
    public class FunctionSignatureFileNode : SignatureFileNode<FunctionSignature>
    {
        public FunctionSignatureFileNode(FunctionSignature signature) : base(signature)
        {
            MatchedSyntax = new SyntaxMatch(AbstractSyntaxElement.FunctionSignature, signature.ParseResult);
        }
    }
}