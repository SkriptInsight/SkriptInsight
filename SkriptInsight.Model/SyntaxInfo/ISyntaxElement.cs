namespace SkriptInsight.Model.SyntaxInfo
{
    public interface ISyntaxElement
    {
        string[] Patterns { get; }

        string AddonName { get; }
    }
}