using System.Diagnostics;

namespace SkriptInsight.Core.SyntaxInfo
{
    [DebuggerDisplay("{" + nameof(ClassName) + "}")]
    public class SkriptCondition : ISyntaxElement
    {
        public string ClassName { get; set; }
        
        public string[] Patterns { get; set; }

        public string AddonName { get; set; }
    }
}