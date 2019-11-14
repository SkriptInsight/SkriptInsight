using System;
using SkriptInsight.Core.Types;

namespace SkriptInsight.Core.Parser.Types
{
    public class SkriptEnumType<T> : SkriptGenericType<SkriptEnumValue<T>> where T: Enum
    {
        protected override SkriptEnumValue<T> TryParse(ParseContext ctx)
        {
            return SkriptEnumValue<T>.TryParse(ctx);
        }

        public override string RenderAsString(SkriptEnumValue<T> obj)
        {
            return obj.ToString();
        }
    }
}