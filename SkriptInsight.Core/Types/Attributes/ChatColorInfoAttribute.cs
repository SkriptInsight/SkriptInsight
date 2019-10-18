using System;

namespace SkriptInsight.Core.Managers.TextDecoration.ChatColoring
{
    [AttributeUsage(AttributeTargets.Field)]
    internal sealed class ChatColorInfoAttribute : Attribute
    {
        public ChatColorInfoAttribute(char character, string color = "", string fontWeight = "",
            string textDecoration = "")
        {
            Character = character;
            Color = color;
            FontWeight = fontWeight;
            TextDecoration = textDecoration;
        }

        public char Character { get; }
        public string Color { get; }
        public string FontWeight { get; }
        public string TextDecoration { get; }
    }
}