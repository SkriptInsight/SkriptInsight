using System;
using SkriptInsight.Core.Parser.Expressions;

namespace SkriptInsight.Core.Parser.Types.Impl.Internal
{
    [InternalType]
    [TypeDescription("si_spaces")]
    public class SkriptMultipleSpaces : ISkriptType
    {
        public IExpression TryParseValue(ParseContext ctx)
        {
            foreach (var c in ctx)
            {
                if (!char.IsWhiteSpace(c))
                    break;
            }
            
            return new Expression<char>(' ');
            
        }

        public string AsString(object obj)
        {
            return "";
        }
    }
}