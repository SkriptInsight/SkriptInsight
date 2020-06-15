using System.Diagnostics;

namespace SkriptInsight.Core.Files.Nodes.Impl.Signatures.ControlFlow.Impl
{
    [ConditionalBasePrefix("loop")]
    [DebuggerDisplay("LoopNode; {ExpressionPattern.ToString()}")]
    [SectionNode]
    public class LoopNodeSignature : ConditionalBaseSignature<LoopNodeSignature>
    {
        public LoopNodeSignature()
        {
        }

        public LoopNodeSignature(LoopNodeSignature signature) : base(signature)
        {
        }
    }
}