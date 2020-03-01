using SkriptInsight.Core.Files.Nodes;
using SkriptInsight.Core.SyntaxInfo;

namespace SkriptInsight.Core.SyntaxNodes
{
    public abstract class BaseSyntaxNode
    {
        public AbstractFileNode Node { get; }
        
        public SyntaxMatch Match { get; }

        public BaseSyntaxNode(AbstractFileNode node, SyntaxMatch match)
        {
            Node = node;
            Match = match;
        }

        public abstract string ToCode();
    }
}