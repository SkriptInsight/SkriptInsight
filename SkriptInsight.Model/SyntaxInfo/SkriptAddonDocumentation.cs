using System.Collections.Generic;

namespace SkriptInsight.Model.SyntaxInfo
{
    public class SkriptAddonDocumentation
    {
        public SkriptAddon Addon { get; set; }

        public List<SkriptType> Types { get; set; }

        public List<SkriptCondition> Conditions { get; set; }

        public List<SkriptEvent> Events { get; set; }

        public List<SkriptExpression> Expressions { get; set; }

        public List<SkriptEffect> Effects { get; set; }
    }
}