using JetBrains.Annotations;
using SkriptInsight.Model.Parser.Expressions;

namespace SkriptInsight.Model.Parser.Types
{
    /// <summary>
    /// Represents a Skript type
    /// </summary>
    public interface ISkriptType
    {
        [CanBeNull]
        IExpression TryParseValue([NotNull] ParseContext ctx);

        string AsString([NotNull] object obj);
    }
}