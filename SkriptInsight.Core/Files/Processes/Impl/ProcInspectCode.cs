using SkriptInsight.Core.Inspections.Impl;
using SkriptInsight.Core.Inspections.Problems;
using SkriptInsight.Core.Managers;

namespace SkriptInsight.Core.Files.Processes.Impl
{
    public class ProcInspectCode : FileProcess
    {
        public ProblemHolder ProblemHolder { get; }

        public ProcInspectCode(ProblemHolder problemHolder)
        {
            ProblemHolder = problemHolder;
        }
        
        public override void DoWork(SkriptFile file, int lineNumber, string rawContent, FileParseContext context)
        {
            foreach (var inspection in WorkspaceManager.Instance.InspectionsManager.CodeInspections.Values)
            {
                if (!inspection.CanInspect(file, lineNumber)) continue;
                
                BaseInspection.ProblemHolder.Value = ProblemHolder;
                inspection.Inspect(file, lineNumber);
                BaseInspection.ProblemHolder.Value = null;
            }
        }
    }
}