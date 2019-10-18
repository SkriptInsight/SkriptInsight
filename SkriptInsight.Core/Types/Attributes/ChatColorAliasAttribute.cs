using System;

namespace SkriptInsight.Core.Types.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    internal sealed class ChatColorAliasAttribute : Attribute
    {
        public ChatColorAliasAttribute(params string[] aliases)
        {
            Aliases = aliases;
        }

        public string[] Aliases { get; }
    }
}