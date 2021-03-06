using System.Threading;
using Humanizer;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Files;
using SkriptInsight.Core.Files.Processes;
using SkriptInsight.Core.Inspections.Problems;

namespace SkriptInsight.Core.Inspections.Impl
{
    /// <summary>
    /// A base code inspection.
    /// </summary>
    public abstract class BaseInspection : FileProcess
    {
        public override string ReportProgressTitle => "Running code inspection";
        public override string ReportProgressMessage => GetType().Name.Replace("Inspection", "").Humanize();

        /// <summary>
        /// A ThreadLocal that holds the ProblemHolder instance for this Inspection
        /// </summary>
        public static ThreadLocal<ProblemsHolder> StaticProblemsHolder { get; } = new ThreadLocal<ProblemsHolder>();

        /// <summary>
        /// The problems holder for this inspection to use.
        /// </summary>
        public ProblemsHolder ProblemsHolder => StaticProblemsHolder.Value;

        /// <summary>
        /// A simple check performed to see if a certain line of a file can be inspected by the current inspection.
        /// </summary>
        /// <param name="file">The file to get inspected</param>
        /// <param name="line">The line number from the file to inspect</param>
        /// <returns></returns>
        public abstract bool CanInspect(SkriptFile file, int line);

        /// <summary>
        /// Inspect the code.
        /// </summary>
        /// <param name="file">The file to get inspected</param>
        /// <param name="line">The line number from the file to inspect</param>
        /// <param name="problemHolder">The instance that holds the problems that are found by this inspection</param>
        public abstract void Inspect(SkriptFile file, int line);

        public sealed override void DoWork(SkriptFile file, int lineNumber, string rawContent, FileParseContext context)
        {
            if (!CanInspect(file, lineNumber)) return;

            StaticProblemsHolder.Value = file.ProblemsHolder;
            Inspect(file, lineNumber);
            StaticProblemsHolder.Value = null;
        }

        protected void AddProblem(DiagnosticSeverity severity, string id, string message, Range range)
        {
            ProblemsHolder.Add(new ProblemDefinition(
                severity,
                id,
                message,
                range.Clone()
            ));
        }
    }
}