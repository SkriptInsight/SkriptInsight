using System.Diagnostics;
using SkriptInsight.Core.Files.Nodes;

namespace SkriptInsight.Core.Parser.Signatures.ControlFlow.Impl
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