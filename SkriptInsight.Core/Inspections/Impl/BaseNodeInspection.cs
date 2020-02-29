using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Files;
using SkriptInsight.Core.Files.Nodes;

namespace SkriptInsight.Core.Inspections.Impl
{
    public abstract class BaseNodeInspection : BaseInspection
    {
        public sealed override bool CanInspect(SkriptFile file, int line)
        {
            return file.Nodes[line] != null && CanInspect(file, line, file.Nodes[line]);
        }

        protected virtual bool CanInspect(SkriptFile file, int line, AbstractFileNode node)
        {
            return true;
        }

        protected abstract void Inspect(SkriptFile file, int line, AbstractFileNode node);

        public sealed override void Inspect(SkriptFile file, int line)
        {
            var node = file.Nodes[line];

            if (node != null) Inspect(file, line, node);
        }
        
        protected void AddProblem(DiagnosticSeverity severity, string message, AbstractFileNode node)
        {
            AddProblem(severity, message, node.ContentRange);
        }

        protected void AddProblem(DiagnosticSeverity severity, string message, Range range)
        {
            AddProblem(severity, string.Intern(GetType().Name.Replace("Inspection", "")), message, range);
        }
    }
}