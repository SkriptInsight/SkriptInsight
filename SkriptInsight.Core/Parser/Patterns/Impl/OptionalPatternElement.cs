namespace SkriptInsight.Core.Parser.Patterns.Impl
{
    [GroupPatternElementInfo('[', ']')]
    public class OptionalPatternElement : AbstractGroupPatternElement
    {
        private AbstractSkriptPatternElement _element;

        public OptionalPatternElement(string contents) : base(contents)
        {
            Element = SkriptPattern.ParsePattern(ParseContext.FromCode(contents));
        }

        public OptionalPatternElement()
        {
        }

        public AbstractSkriptPatternElement Element
        {
            get => _element;
            set
            {
                if (value is SkriptPattern sp) sp.FastFail = true;
                _element = value;
            }
        }

        public override ParseResult Parse(ParseContext ctx)
        {
            var oldPos = ctx.CurrentPosition;
            ctx.StartMatch();
            var parseResult = Element.Parse(ctx);

            if (parseResult.IsSuccess)
            {
                ctx.EndMatch(true);
                return parseResult;
            }

            ctx.UndoMatch();
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