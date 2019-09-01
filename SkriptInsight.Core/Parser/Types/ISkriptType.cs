using JetBrains.Annotations;
using SkriptInsight.Core.Parser.Expressions;

namespace SkriptInsight.Core.Parser.Types
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