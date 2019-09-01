namespace SkriptInsight.Core.Parser.Expressions.Variables.Content
{
    public class SplitLiteralContent : VariableContent
    {
        public VariableContent Content { get; set; }

        public SplitLiteralContent(VariableContent content) : base(content.RenderContent())
        {
            Content = content;
        }

        public override string RenderContent()
        {
            return $"::{Content.RenderContent()}";
        }
    }
}