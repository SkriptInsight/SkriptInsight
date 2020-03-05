using System.Text.RegularExpressions;
using SkriptInsight.Core.Extensions;
  
namespace SkriptInsight.Core.Parser.Patterns.Impl
{
    [GroupPatternElementInfo('<', '>')]
    public class RegexMatchPatternElement : AbstractGroupPatternElement
    {
        private string _expression;

        public string Expression
        {
            get => _expression;
            set
            {
                _expression = value;
                ExpressionRegex = new Regex(_expression, RegexOptions.Compiled);
            }
        }

        public Regex ExpressionRegex { get; set; }

        public RegexMatchPatternElement()
        {
        }

        public RegexMatchPatternElement(string contents) : base(contents)
        {
            Expression = contents;
        }

        public override ParseResult Parse(ParseContext ctx)
        {
            var untilEnd = ctx.PeekUntilEnd();
            var limit = untilEnd.IndexOfAny(new[] {'(', ')', '{', '}', '[', ']'});

            var text = limit > -1 ? untilEnd.SafeSubstring(0, limit) : untilEnd;
            ctx.StartRangeMeasure();
            var match = ExpressionRegex.Match(text);
            
            if (!match.Success)
            {
                ctx.UndoRangeMeasure();
                return ParseResult.Failure(ctx);
            }

            ctx.ReadUntilPosition(ctx.CurrentPosition + match.Index + match.Length);

            var parseMatch = new ParseMatch
            {
                Context = ctx,
                Range = ctx.EndRangeMeasure(),
                RawContent = match.Value,
                ElementInfo = ParseMatch.LoadElementInfo(this)
            };
            ctx.Matches.Add(parseMatch);
            
            return ParseResult.Success(ctx);
        }

        public override string RenderPattern()
        {
            return $"<{Expression}>";
        }
    }
}