namespace SkriptInsight.Core.Files.Nodes.Impl.Signatures.ControlFlow.Impl
{
    [ConditionalBasePrefix("while")]
    public class WhileNodeSignature : ConditionalBaseSignature<WhileNodeSignature>
    {
        public WhileNodeSignature()
        {
        }

        public WhileNodeSignature(WhileNodeSignature signature) : base(signature)
        {
        }
    }
}