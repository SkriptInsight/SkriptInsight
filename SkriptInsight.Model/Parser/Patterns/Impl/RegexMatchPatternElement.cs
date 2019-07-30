namespace SkriptInsight.Model.Parser.Patterns.Impl
{
    [GroupPatternElementInfo('<', '>')]
    public class RegexMatchPatternElement : AbstractGroupPatternElement
    {
        public string Expression { get; }

        public RegexMatchPatternElement(string contents) : base(contents)
        {
            Expression = contents;
        }

        public override ParseResult Parse(ParseContext ctx)
        {
            return ParseResult.Failure(ctx);
        }

        public override string RenderPattern()
        {
            return $"<{Expression}>";
        }
    }
}