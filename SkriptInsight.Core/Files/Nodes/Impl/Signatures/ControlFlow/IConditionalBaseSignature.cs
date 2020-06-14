using SkriptInsight.Core.Parser.Expressions;

namespace SkriptInsight.Core.Files.Nodes.Impl.Signatures.ControlFlow
{
    public interface IConditionalBaseSignature
    {
        IExpression ConditionExpression { get; }
    }
}