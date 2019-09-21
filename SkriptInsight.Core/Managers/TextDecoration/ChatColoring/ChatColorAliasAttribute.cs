using System;

namespace SkriptInsight.Core.Managers.TextDecoration.ChatColoring
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