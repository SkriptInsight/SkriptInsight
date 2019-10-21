using System.Linq;
using SkriptInsight.Core.Parser.Patterns.Impl;
using SkriptInsight.Core.SyntaxInfo;
using static SkriptInsight.Core.Managers.WorkspaceManager;

namespace SkriptInsight.Core.Parser.Types.Impl
{
    [TypeDescription("classinfo")]
    public class SkriptClassInfo : SkriptGenericType<SkriptType>
    {
        protected override SkriptType TryParse(ParseContext ctx)
        {
            var element = new LiteralPatternElement("");
            var startPos = ctx.CurrentPosition;
            var clone = ctx.Clone();

            foreach (var type in CurrentWorkspace.AddonDocumentations.SelectMany(c => c.Types))
            {
                clone.CurrentPosition = startPos;
                element.Value = type.TypeName;
                var result = element.Parse(clone);

                if (!result.IsSuccess) continue;
                ctx.CurrentPosition = clone.CurrentPosition;
                return type;
            }

            return null;
        }

        public override string AsString(SkriptType obj)
        {
            return obj.TypeName;
        }
    }
}