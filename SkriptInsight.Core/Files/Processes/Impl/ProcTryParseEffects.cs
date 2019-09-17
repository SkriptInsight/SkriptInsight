using System.Linq;
using SkriptInsight.Core.Managers;
using SkriptInsight.Core.SyntaxInfo;

namespace SkriptInsight.Core.Files.Processes.Impl
{
    public class ProcTryParseEffects : FileProcess
    {
        public override void DoWork(SkriptFile file, int lineNumber, string rawContent, FileParseContext context)
        {
            var node = file.Nodes[lineNumber];
            var workDone = false;
            foreach (var elements in WorkspaceManager.Instance.Current.AddonDocumentations.Select(addon => node.IsSectionNode ? addon.Events.Cast<AbstractSyntaxElement>() : addon.Effects))
            {
                foreach (var effect in elements)
                {
                    foreach (var effectPattern in effect.PatternNodes)
                    {
                        context.CurrentLine = lineNumber;
                        var result = effectPattern.Parse(context);
                        if (result.IsSuccess)
                        {
                            node.MatchedSyntax = new SyntaxMatch(effect, result);
                            workDone = true;
                        }

                        if (workDone) break;
                    }

                    if (workDone) break;
                }
                if (workDone) break;
            }
            
            
        }
    }
}