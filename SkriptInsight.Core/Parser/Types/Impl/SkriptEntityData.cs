using System.Linq;
using SkriptInsight.Core.Managers;
using SkriptInsight.Core.Types;

namespace SkriptInsight.Core.Parser.Types.Impl
{
    [TypeDescription("entitydata")]
    public class SkriptEntityData : SkriptGenericType<EntityData>
    {
        protected override EntityData TryParse(ParseContext ctx)
        {
            var clone = ctx.Clone(false);
            var pos = clone.CurrentPosition;

            foreach (var type in WorkspaceManager.CurrentWorkspace.AddonDocumentations.SelectMany(c => c.Types)
                .Where(c => c.PossibleValuesAsNouns != null))
            {
                foreach (var noun in type.PossibleValuesAsNouns)
                {
                    var result = noun.Pattern.Parse(clone);
                    if (result.IsSuccess)
                    {
                        ctx.ReadUntilPosition(clone.CurrentPosition);
                        var data = new EntityData(noun, (EntityData.EntityTypeUsedValues)result.ParseMark);
                        return data;
                    }

                    clone.CurrentPosition = pos;
                    clone.Matches.Clear();
                }
            }

            return null;
        }

        public override string AsString(EntityData obj)
        {
            return obj.ToString();
        }
    }
}