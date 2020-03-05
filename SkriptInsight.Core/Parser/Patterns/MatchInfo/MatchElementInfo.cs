using System.Diagnostics;

namespace SkriptInsight.Core.Parser.Patterns.MatchInfo
{
    /// <summary>
    /// A class that holds the information about what pattern element matched a parse match 
    /// </summary>
    [DebuggerDisplay("{Type} @ {Index}")]
    public class MatchElementInfo
    {
        public int Index { get; set; }

        public ElementType Type { get; set; }
    }
}