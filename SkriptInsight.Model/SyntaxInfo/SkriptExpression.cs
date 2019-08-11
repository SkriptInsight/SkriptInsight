namespace SkriptInsight.Model.SyntaxInfo
{
    public class SkriptExpression : ISyntaxElement
    {
        public string ClassName { get; set; }

        public string[] Patterns { get; set; }

        public string AddonName { get; set; }

        public ExpressionType ExpressionType { get; set; }
    }
}