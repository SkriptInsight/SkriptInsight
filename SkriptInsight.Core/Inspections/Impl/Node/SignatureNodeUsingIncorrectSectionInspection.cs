using System.Reflection;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Files;
using SkriptInsight.Core.Files.Nodes;
using SkriptInsight.Core.Parser.Expressions;

namespace SkriptInsight.Core.Inspections.Impl.Node
{
    [InspectionDescriptionAttribute]
    public class SignatureNodeUsingIncorrectSectionInspection : BaseNodeInspection
    {
        protected override bool CanInspect(SkriptFile file, int line, AbstractFileNode node)
        {
            return node.MatchedSyntax == null;
        }

        protected override void Inspect(SkriptFile file, int line, AbstractFileNode node)
        {
            if (node.GetType().IsSubclassOfRawGeneric(typeof(SignatureFileNode<>)))
            {
                var signatureRequiresSection = node.GetType().GetCustomAttribute<SectionNodeAttribute>() != null;
                var isSectionMismatch = node.IsSectionNode !=
                                        signatureRequiresSection;

                if (isSectionMismatch)
                {
                    AddProblem(DiagnosticSeverity.Error,
                        $"{(signatureRequiresSection ? "E" : "Une")}xpected section at the end of this line", node);
                }
            }
        }
    }
}