namespace SkriptInsight.Core.Parser.Expressions.Variables
{
    public abstract class VariableContent
    {
        protected VariableContent(string content)
        {
            RawContent = content;
        }
        public string RawContent { get; }

        public abstract string RenderContent();
    }
}