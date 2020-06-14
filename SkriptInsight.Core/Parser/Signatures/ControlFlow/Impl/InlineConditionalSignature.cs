using System.Diagnostics;

namespace SkriptInsight.Core.Parser.Signatures.ControlFlow.Impl
{
    [ConditionalBasePrefix("")]
    [DebuggerDisplay("Inline Conditional; {RawText}")]
    public class InlineConditionalSignature : ConditionalBaseSignature<InlineConditionalSignature>
    {
        public InlineConditionalSignature()
        {
        }

        public InlineConditionalSignature(InlineConditionalSignature signature) : base(signature)
        {
        }
    }
}