namespace SkriptInsight.Core.Files.Nodes.Impl.Signatures.ControlFlow.Impl
{
    [ConditionalBasePrefix("loop")]
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