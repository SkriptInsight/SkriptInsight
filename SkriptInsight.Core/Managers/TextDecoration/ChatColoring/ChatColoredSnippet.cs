using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Types;

namespace SkriptInsight.Core.Managers.TextDecoration.ChatColoring
{
    public class ChatColoredSnippet
    {
        public Range Range { get; set; }

        public string Text { get; set; }

        public ChatColor Color { get; set; } = ChatColor.Reset;
    }
}