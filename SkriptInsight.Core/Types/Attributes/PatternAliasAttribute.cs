using System;

namespace SkriptInsight.Core.Types.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    internal sealed class PatternAliasAttribute : Attribute
    {
        public PatternAliasAttribute(params string[] aliases)
        {
            Aliases = aliases;
        }

        public string[] Aliases { get; }
    }
}