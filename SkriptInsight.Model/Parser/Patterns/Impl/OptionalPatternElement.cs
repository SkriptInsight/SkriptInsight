namespace SkriptInsight.Model.Parser.Patterns.Impl
{
    public class OptionalPatternElement : AbstractSkriptPatternElement
    {
        public AbstractSkriptPatternElement Element { get; set; }

        public OptionalPatternElement(AbstractSkriptPatternElement element)
        {
            Element = element;
        }
        
        public override PatternParseResult Parse(ParseContext ctx)
        {
            var parseResult = Element.Parse(ctx);

            if (parseResult.IsSuccess) return parseResult;
            parseResult.ResultType = PatternParseResultType.Success;
            parseResult.IsOptionallyMatched = true;

            return parseResult;
        }

        public override string RenderPattern()
        {
            return $"[{Element.RenderPattern()}]";
        }
    }
}