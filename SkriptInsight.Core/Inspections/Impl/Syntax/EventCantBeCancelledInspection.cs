using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Files;
using SkriptInsight.Core.Files.Nodes;
using SkriptInsight.Core.SyntaxInfo;
using SkriptInsight.Core.SyntaxNodes;
using SkriptInsight.Core.SyntaxNodes.Impl;

namespace SkriptInsight.Core.Inspections.Impl.Syntax
{
    [InspectionDescriptionAttribute]
    public class EventCantBeCancelledInspection : BaseSyntaxInspection
    {
        protected override void Inspect(SkriptFile file, int line, AbstractFileNode node, SyntaxMatch match)
        {
            var cancelEvent = node.GetSyntaxNode<EffCancelEvent>();
            if (cancelEvent == null) return;

            if (node.RootParentSyntax?.Element is SkriptEvent @event && !@event.Cancellable)
            {
                AddProblem(DiagnosticSeverity.Error, $"This event can't be {(!cancelEvent.ToCancel ? "un-" : "")}cancelled.", node);
            }
        }
    }
}