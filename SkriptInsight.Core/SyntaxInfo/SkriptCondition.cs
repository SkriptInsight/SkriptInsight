using System.Diagnostics;

namespace SkriptInsight.Core.SyntaxInfo
{
    [DebuggerDisplay("{" + nameof(ClassName) + "}")]
    public class SkriptCondition : AbstractSyntaxElement
    {
        public static readonly SkriptCondition TrueLiteral = new SkriptCondition
        {
            ClassName = "Boolean",
            Patterns = new[]
            {
                "(true|yes|on)"
            },
            AddonName = "Skript"
        };

        public static readonly SkriptCondition FalseLiteral = new SkriptCondition
        {
            ClassName = "Boolean",
            Patterns = new[]
            {
                "(false|no|off)"
            },
            AddonName = "Skript"
        };

        public string ClassName { get; set; }
    }
}