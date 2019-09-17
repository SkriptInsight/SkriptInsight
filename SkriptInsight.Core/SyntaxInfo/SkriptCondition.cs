using System.Diagnostics;

namespace SkriptInsight.Core.SyntaxInfo
{
    [DebuggerDisplay("{" + nameof(ClassName) + "}")]
    public class SkriptCondition : AbstractSyntaxElement
    {
        public string ClassName { get; set; }
    }
}