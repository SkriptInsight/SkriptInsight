using System.Collections.Generic;

namespace SkriptInsight.Core.Parser.Patterns
{
    public class ParseResult
    {
        public ParseResultType ResultType { get; set; }

        public ParseContext? Context { get; set; }

        public AbstractSkriptPatternElement? MatchedElement { get; set; }

        public bool IsSuccess => ResultType == ParseResultType.Success;

        public int ParseMark { get; set; }

        public bool IsOptionallyMatched { get; set; }

        public List<ParseMatch> Matches { get; set; } = new List<ParseMatch>();

        public static ParseResult Success(ParseContext ctx)
        {
            return new ParseResult
            {
                MatchedElement = ctx.ElementContext?.Current,
                ResultType = ParseResultType.Success,
                Context = ctx,
                Matches = ctx.Matches
            };
        }

        public static ParseResult OptionalSuccess(ParseContext ctx)
        {
            var result = Success(ctx);
            result.IsOptionallyMatched = true;
            return result;
        }

        public static ParseResult Failure(ParseContext ctx)
        {
            return new ParseResult
            {
                MatchedElement = ctx.ElementContext?.Current,
                ResultType = ParseResultType.Failure,
                Context = ctx,
                Matches = ctx.Matches
            };
        }
    }
}