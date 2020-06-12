using System.Linq;
using SkriptInsight.Core.Parser.Expressions.Variables;

namespace SkriptInsight.Core.Parser.Types.Impl
{
    public class SkriptVariableReferenceType : SkriptGenericType<SkriptVariable>
    {
        protected override SkriptVariable TryParse(ParseContext ctx)
        {
            var clone = ctx.Clone();

            if (clone.PeekNext(1) == "{")
            {
                var result = SkriptVariable.TryParse(clone);

                if (result != null) ctx.ReadUntilPosition(clone.CurrentPosition);

                if (ctx.ReadNext(1) == "}")
                    return result;
            }

            return null;
        }

        public override string AsString(SkriptVariable obj)
        {
            return string.Join("", obj.Contents.Select(c => c.RenderContent()));
        }
    }
}