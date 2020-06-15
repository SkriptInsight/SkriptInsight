using System.Diagnostics;

namespace SkriptInsight.Core.Files.Nodes.Impl.Signatures.ControlFlow.Impl
{
    [ConditionalBasePrefix("while")]
    [DebuggerDisplay("WhileNode; {ExpressionPattern.ToString()}")]
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