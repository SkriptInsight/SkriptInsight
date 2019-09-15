using SkriptInsight.Core.Managers;

namespace SkriptInsight.Core.Files.Processes.Impl
{
    public class ProcTryParseEffects : FileProcess
    {
        public override void DoWork(SkriptFile file, int lineNumber, string rawContent, FileParseContext context)
        {
            var workDone = false;
            foreach (var addon in WorkspaceManager.Instance.Current.AddonDocumentations)
            {
                foreach (var effect in addon.Effects)
                {
                    foreach (var effectPattern in effect.PatternNodes)
                    {
                        context.CurrentLine = lineNumber;
                        var result = effectPattern.Parse(context);
                        if (result.IsSuccess)
                        {
                            file.Nodes[lineNumber].ParseResult = result;
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