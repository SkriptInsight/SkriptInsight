using SkriptInsight.Core.Files.Nodes;

namespace SkriptInsight.Core.Parser.Signatures.ControlFlow.Impl
{
    [SectionNode, ConditionalBasePrefix("")]
    public class DiscountIfNodeSignature : ConditionalBaseSignature<DiscountIfNodeSignature>
    {
        public DiscountIfNodeSignature()
        {
        }

        public DiscountIfNodeSignature(DiscountIfNodeSignature signature) : base(signature)
        {
        }
    }
}