using SkriptInsight.Core.Files.Nodes;

namespace SkriptInsight.Core.Parser.Signatures.ControlFlow.Impl
{
    [SectionNode, ConditionalBasePrefix("if")]
    public class IfNodeSignature : ConditionalBaseSignature<IfNodeSignature>
    {
        public IfNodeSignature(IfNodeSignature signature) : base(signature)
        {
        }

        public IfNodeSignature()
        {
        }
    }
}