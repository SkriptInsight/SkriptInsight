using System.Diagnostics;
using System.Linq;
using SkriptInsight.Core.Files.Nodes;
using SkriptInsight.Core.SyntaxInfo;

namespace SkriptInsight.Core.SyntaxNodes.Impl
{
    [SyntaxInfo("ch.njol.skript.effects.EffCancelEvent")]
    public class EffCancelEvent : BaseSyntaxNode
    {
        public bool ToCancel { get; set; }
        
        public bool ContainsDefiniteArticle { get; set; }
        
        public EffCancelEvent(AbstractFileNode node, SyntaxMatch match) : base(node, match)
        {
            ToCancel = match.PatternIndex == 0;
            ContainsDefiniteArticle = match.Result.Context.Matches.Any(c => c.RawContent.ToLower() == "the");
        }

        public override string ToCode()
        {
            return $"{(ToCancel ? "un" : "")}cancel{(ContainsDefiniteArticle ? " the" : "")} event";
        }
    }
}