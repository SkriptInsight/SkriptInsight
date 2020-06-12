using SkriptInsight.Core.Files;
using SkriptInsight.Core.Files.Nodes;
using SkriptInsight.Core.SyntaxInfo;

namespace SkriptInsight.Core.Inspections.Impl
{
    public abstract class BaseSyntaxInspection : BaseNodeInspection
    {
        protected sealed override bool CanInspect(SkriptFile file, int line, AbstractFileNode node)
        {
            return node.MatchedSyntax != null && node.MatchedSyntax.Result.IsSuccess
                                              && CanInspect(file, line, node, node.MatchedSyntax);
        }

        protected virtual bool CanInspect(SkriptFile file, int line, AbstractFileNode node, SyntaxMatch match)
        {
            return true;
        }

        protected abstract void Inspect(SkriptFile file, int line, AbstractFileNode node, SyntaxMatch match);

        protected sealed override void Inspect(SkriptFile file, int line, AbstractFileNode node)
        {
            if (node.MatchedSyntax != null)
                Inspect(file, line, node, node.MatchedSyntax);
        }
    }
}