using JetBrains.Annotations;
using SkriptInsight.Core.Managers;

namespace SkriptInsight.Core.SyntaxInfo
{
    [UsedImplicitly]
    public class EventValueInfo
    {
        public string ValueName { get; set; }

        public string ValueClass { get; set; }

        public SkriptExpression ToEventExpression(SkriptTypesManager manager, SkriptEvent skriptEvent)
        {
            return new SkriptEventValueExpression(this, skriptEvent)
            {
                AddonName = skriptEvent.AddonName,
                Patterns = new[] {ValueName}
            };
        }
    }
}