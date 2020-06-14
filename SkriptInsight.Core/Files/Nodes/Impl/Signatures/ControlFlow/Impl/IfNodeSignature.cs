using System.Diagnostics;

namespace SkriptInsight.Core.Files.Nodes.Impl.Signatures.ControlFlow.Impl
{
    [SectionNode, ConditionalBasePrefix("if")]
    [DebuggerDisplay("Inline Conditional; {RawText}")]
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