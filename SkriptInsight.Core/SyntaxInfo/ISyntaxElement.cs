namespace SkriptInsight.Core.SyntaxInfo
{
    public interface ISyntaxElement
    {
        string[] Patterns { get; }

        string AddonName { get; }
    }
}