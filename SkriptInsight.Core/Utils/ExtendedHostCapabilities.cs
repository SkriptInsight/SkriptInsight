using JetBrains.Annotations;

namespace SkriptInsight.Core.Utils
{
    [UsedImplicitly]
    public class ExtendedHostCapabilities
    {
        /// <summary>
        /// Whether this host supports reporting of current viewport range.
        /// Used to optimize the parsing process.
        /// </summary>
        public bool SupportsViewportReporting { get; set; }
    }
}