namespace SkriptInsight.Model.Parser.Types.Impl
{
    [TypeDescription("string")]
    public class SkriptString : SkriptType<string>
    {
        protected override Expression<string> ParseExpression(ParseContext ctx)
        {
            ctx.StartMatch();
            if (ctx.ReadNext(1) == "\"")
            {
                ctx.StartRangeMeasure("String content");

                var closingPos = ctx.FindNextBracket('"', true);
                if (closingPos > -1)
                {
                    var value = ctx.ReadUntilPosition(closingPos);
                    if (ctx.ReadNext(1) == "\"")
                    {
                        var contentRange = ctx.EndRangeMeasure("String content");
                        return new Expression<string>(value, ctx.EndMatch(), contentRange);
                    }
                }
            }

            ctx.UndoRangeMeasure();
            ctx.UndoMatch();
            return null;
        }
    }
}