namespace SkriptInsight.Core.Parser.Signatures.ControlFlow
{
    [ConditionalBasePrefix("if")]
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