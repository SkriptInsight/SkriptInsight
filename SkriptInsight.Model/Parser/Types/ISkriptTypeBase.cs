namespace SkriptInsight.Model.Parser.Types
{
    public interface ISkriptTypeBase
    {
        IExpression Parse(ParseContext ctx);
    }
}