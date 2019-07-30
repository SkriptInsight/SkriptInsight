namespace SkriptInsight.Model.Parser.Patterns
{
    public class PatternParseResult
    {
        public PatternParseResultType ResultType { get; set; }

        public ParseContext Context { get; set; }

        public bool IsSuccess => ResultType == PatternParseResultType.Success;

        public int ParseMark { get; set; } = 0;
        
        public bool IsOptionallyMatched { get; set; }

        public static PatternParseResult Success(ParseContext ctx)
        {
            return new PatternParseResult
            {
                ResultType = PatternParseResultType.Success,
                Context = ctx
            };
        }
        
        public static PatternParseResult Failure(ParseContext ctx)
        {
            return new PatternParseResult
            {
                ResultType = PatternParseResultType.Failure,
                Context = ctx
            };
        }
    }
}