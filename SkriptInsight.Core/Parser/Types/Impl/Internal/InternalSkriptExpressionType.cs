using SkriptInsight.Core.Parser.Expressions;

namespace SkriptInsight.Core.Parser.Types.Impl.Internal
{
    [InternalType]
    [TypeDescription("si_expression")]
    public class InternalSkriptExpressionType : ISkriptType
    {
        private InternalSkriptExpressionType()
        {
        }
        public static InternalSkriptExpressionType Instance = new InternalSkriptExpressionType();
        
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