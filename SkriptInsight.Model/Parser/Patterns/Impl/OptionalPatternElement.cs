namespace SkriptInsight.Model.Parser.Patterns.Impl
{
    [GroupPatternElementInfo('[', ']')]
    public class OptionalPatternElement : AbstractGroupPatternElement
    {
        public OptionalPatternElement(string contents) : base(contents)
        {
            Element = SkriptPattern.ParsePattern(ParseContext.FromCode(contents));
        }

        public AbstractSkriptPatternElement Element { get; set; }

        public override ParseResult Parse(ParseContext ctx)
        {
            var oldPos = ctx.CurrentPosition;
            var parseResult = Element.Parse(ctx);

            if (parseResult.IsSuccess) return parseResult;
            ctx.CurrentPosition = oldPos;
            parseResult.ResultType = ParseResultType.Success;
            parseResult.IsOptionallyMatched = true;

            return parseResult;
        }

        public override string RenderPattern()
        {
            return $"[{Element.RenderPattern()}]";
        }
    }
}