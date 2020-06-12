namespace SkriptInsight.Core.Parser.Types.Impl
{
    [TypeDescription("string")]
    public class SkriptString : SkriptGenericType<string>
    {
        protected override string TryParse(ParseContext ctx)
        {
            if (ctx.PeekNext(1) != "\"") return null;
            ctx.ReadNext(1); //Read open quote
            var closingPos = ctx.FindNextBracket('"', true);
            if (closingPos <= -1) return null;
            var value = ctx.ReadUntilPosition(closingPos);
            return ctx.ReadNext(1) == "\"" ? value.Replace("\"\"", "\"") : null;
        }

        public override string AsString(string obj)
        {
            return $"\"{obj.Replace("\"", "\"\"")}\"";
        }
    }
}