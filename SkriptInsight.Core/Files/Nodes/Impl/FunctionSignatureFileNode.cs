using JetBrains.Annotations;
using SkriptInsight.Core.Parser.Functions;

namespace SkriptInsight.Core.Files.Nodes.Impl
{
    [UsedImplicitly]
    public class FunctionSignatureFileNode : SignatureFileNode<FunctionSignature>
    {
        public FunctionSignatureFileNode(FunctionSignature signature) : base(signature)
        {
        }
    }
}