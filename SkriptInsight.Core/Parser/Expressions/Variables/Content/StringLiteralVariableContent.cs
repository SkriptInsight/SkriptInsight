namespace SkriptInsight.Core.Parser.Expressions.Variables.Content
{
    public class StringLiteralVariableContent : VariableContent
    {
        public StringLiteralVariableContent(string content) : base(content)
        {
        }

        public override string RenderContent()
        {
            return RawContent;
        }
    }
}