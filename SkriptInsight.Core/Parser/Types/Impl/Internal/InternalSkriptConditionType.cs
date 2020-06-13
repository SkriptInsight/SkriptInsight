using SkriptInsight.Core.Parser.Expressions;

namespace SkriptInsight.Core.Parser.Types.Impl.Internal
{
    [InternalType]
    [TypeDescription("si_condition")]
    public class InternalSkriptConditionType : ISkriptType
    {
        public static InternalSkriptConditionType Instance { get; } = new InternalSkriptConditionType();
        
        public IExpression TryParseValue(ParseContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public string AsString(object obj)
        {
            throw new System.NotImplementedException();
        }
    }
}