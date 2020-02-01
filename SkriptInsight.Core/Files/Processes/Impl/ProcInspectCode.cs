using SkriptInsight.Core.Inspections.Problems;
using SkriptInsight.Core.Managers;

namespace SkriptInsight.Core.Files.Processes.Impl
{
    public class ProcInspectCode : FileProcess
    {
        public override void DoWork(SkriptFile file, int lineNumber, string rawContent, FileParseContext context)
        {
            foreach (var inspection in WorkspaceManager.Instance.InspectionsManager.CodeInspections)
            {
                var holder = new ProblemHolder();
                if (inspection.Value.CanInspect(file, lineNumber))
                {
                    inspection.Value.Inspect(file, lineNumber, holder);
                }
            }
        }
    }
}