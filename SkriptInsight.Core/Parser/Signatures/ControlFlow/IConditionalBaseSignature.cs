using SkriptInsight.Core.Parser.Expressions;

namespace SkriptInsight.Core.Parser.Signatures.ControlFlow
{
    public interface IConditionalBaseSignature
    {
        IExpression ConditionExpression { get; }
    }
}