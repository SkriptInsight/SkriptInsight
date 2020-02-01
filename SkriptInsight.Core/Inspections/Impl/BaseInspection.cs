using System.Threading;
using SkriptInsight.Core.Files;
using SkriptInsight.Core.Inspections.Problems;

namespace SkriptInsight.Core.Inspections.Impl
{
    /// <summary>
    /// A base code inspection.
    /// </summary>
    public abstract class BaseInspection
    {
        /// <summary>
        /// A ThreadLocal that holds the ProblemHolder instance for this Inspection
        /// </summary>
        public static ThreadLocal<ProblemHolder> ProblemHolder { get; } = new ThreadLocal<ProblemHolder>();

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
    }
}