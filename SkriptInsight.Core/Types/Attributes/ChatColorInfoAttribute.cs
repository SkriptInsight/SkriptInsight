using System;

namespace SkriptInsight.Core.Types.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    internal sealed class ChatColorInfoAttribute : Attribute
    {
        public ChatColorInfoAttribute(char character, string color = "", string unsaturatedColor = "",
            string fontWeight = "",
            string textDecoration = "")
        {
            Character = character;
            Color = color;
            UnsaturatedColor = unsaturatedColor;
            FontWeight = fontWeight;
            TextDecoration = textDecoration;
        }

        public char Character { get; }
        public string Color { get; }

        public string UnsaturatedColor { get; }
        public string FontWeight { get; }
        public string TextDecoration { get; }
    }
}