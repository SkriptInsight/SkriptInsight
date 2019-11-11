using System.Diagnostics;

namespace SkriptInsight.Core.Files.Nodes.Impl
{
    [DebuggerDisplay("Empty line")]
    public class EmptyLineNode : AbstractFileNode
    {
        public override string ToString()
        {
            return "Empty line";
        }
    }
}