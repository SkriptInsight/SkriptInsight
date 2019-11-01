using System;
using SkriptInsight.Core.Parser.Expressions;

namespace SkriptInsight.Core.Parser.Types.Impl.Internal
{
    [TypeDescription("si_void")]
    [InternalType]
    public class SkriptVoid : ISkriptType
    {
        public IExpression TryParseValue(ParseContext ctx)
        {
            return null;
        }

        public string AsString(object obj)
        {
            return "void";
        }
    }
}