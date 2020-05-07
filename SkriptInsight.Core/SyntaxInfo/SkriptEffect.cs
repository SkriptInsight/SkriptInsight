using System.Diagnostics;

namespace SkriptInsight.Core.SyntaxInfo
{
    [DebuggerDisplay("{" + nameof(ClassName) + "}")]
    public class SkriptEffect : AbstractSyntaxElement
    {
        public string ClassName { get; set; }
    }
}