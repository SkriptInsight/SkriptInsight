namespace SkriptInsight.Core.Parser.Expressions.Variables.Content
{
    public class ExpressionVariableContent : VariableContent
    {
        public ExpressionVariableContent(string content) : base(content)
        {
        }

        public override string RenderContent()
        {
            return $"%{RawContent}%";
        }
    }
}