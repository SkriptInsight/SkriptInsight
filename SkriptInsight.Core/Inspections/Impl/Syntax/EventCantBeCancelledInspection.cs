using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Files;
using SkriptInsight.Core.Files.Nodes;
using SkriptInsight.Core.SyntaxInfo;

namespace SkriptInsight.Core.Inspections.Impl.Syntax
{
    [InspectionDescriptionAttribute]
    public class EventCantBeCancelledInspection : BaseSyntaxInspection
    {
        protected override void Inspect(SkriptFile file, int line, AbstractFileNode node, SyntaxMatch match)
        {
            if (!node.IsMatchOfType(x => x.EffCancelEvent)) return;
            
            if (node.RootParentSyntax?.Element is SkriptEvent @event && !@event.Cancellable)
            {
                AddProblem(DiagnosticSeverity.Error, "This event can't be cancelled", node);
            }
        }
    }
}