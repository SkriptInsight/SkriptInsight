using System.Diagnostics;

namespace SkriptInsight.Core.SyntaxInfo
{
    [DebuggerDisplay("{" + nameof(ClassName) + "}")]
    public class SkriptExpression : AbstractSyntaxElement
    {
        public string ClassName { get; set; }

        public ExpressionType ExpressionType { get; set; }
    }
}