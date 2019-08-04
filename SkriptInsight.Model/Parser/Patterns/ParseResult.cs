using System.Collections.Generic;

namespace SkriptInsight.Model.Parser.Patterns
{
    public class ParseResult
    {
        public ParseResultType ResultType { get; set; }

        public ParseContext? Context { get; set; }

        public bool IsSuccess => ResultType == ParseResultType.Success;

        public int ParseMark { get; set; }
        
        public bool IsOptionallyMatched { get; set; } 

        public List<ParseMatch> Matches { get; set; } = new List<ParseMatch>();
        
        public static ParseResult Success(ParseContext ctx)
        {
            return new ParseResult
            {
                ResultType = ParseResultType.Success,
                Context = ctx,
                Matches = ctx.Matches
            };
        }
        
        public static ParseResult Failure(ParseContext ctx)
        {
            return new ParseResult
            {
                ResultType = ParseResultType.Failure,
                Context = ctx,
                Matches = ctx.Matches
            };
        }
    }
}