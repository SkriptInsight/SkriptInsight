using System.Diagnostics;

namespace SkriptInsight.Core.Files.Nodes.Impl.Signatures.ControlFlow.Impl
{
    [SectionNode, ConditionalBasePrefix("if", true)]
    [DebuggerDisplay("IfNode; {ExpressionPattern.ToString()}")]
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