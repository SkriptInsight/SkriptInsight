namespace SkriptInsight.Model.SyntaxInfo
{
    public class SkriptCondition : ISyntaxElement
    {
        public string ClassName { get; set; }
        public string[] Patterns { get; set; }

        public string AddonName { get; set; }
    }
}