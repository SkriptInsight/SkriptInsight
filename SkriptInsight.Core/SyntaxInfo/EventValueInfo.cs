using JetBrains.Annotations;
using SkriptInsight.Core.Managers;

namespace SkriptInsight.Core.SyntaxInfo
{
    [UsedImplicitly]
    public class EventValueInfo
    {
        public string ValueName { get; set; }

        public string ValueClass { get; set; }

        public SyntaxSkriptExpression ToEventExpression(SkriptTypesManager manager, SkriptEvent skriptEvent)
        {
            return new SyntaxSkriptEventValueExpression(this, skriptEvent)
            {
                AddonName = skriptEvent.AddonName,
                Patterns = new[] {ValueName}
            };
        }
    }
}