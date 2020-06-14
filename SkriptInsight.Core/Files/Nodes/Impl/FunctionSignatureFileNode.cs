using SkriptInsight.Core.Files.Nodes.Impl.Signatures.Functions;
using SkriptInsight.Core.SyntaxInfo;

namespace SkriptInsight.Core.Files.Nodes.Impl
{
    [SectionNode]
    public class FunctionSignatureFileNode : SignatureFileNode<FunctionSignature>
    {
        public FunctionSignatureFileNode(FunctionSignature signature) : base(signature)
        {
            MatchedSyntax = new SyntaxMatch(SignatureElements.FunctionSignature, signature.ParseResult);
        }
    }
}