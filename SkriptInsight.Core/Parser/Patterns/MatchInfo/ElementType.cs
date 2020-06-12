using System;
using SkriptInsight.Core.Parser.Patterns.Impl;

namespace SkriptInsight.Core.Parser.Patterns.MatchInfo
{
    public enum ElementType
    {
        None,
        Choice,
        Optional,
        Regex,
        Type
    }

    public static class ElementTypeExtensions
    {
        public static Type ToClassType(this ElementType type)
        {
            switch (type)
            {
                case ElementType.Choice:
                    return typeof(ChoicePatternElement);
                case ElementType.Optional:
                    return typeof(OptionalPatternElement);
                case ElementType.Regex:
                    return typeof(RegexMatchPatternElement);
                case ElementType.Type:
                    return typeof(TypePatternElement);
                default:
                    return typeof(AbstractSkriptPatternElement);
            }
        }

        public static ElementType ToElementType(this AbstractSkriptPatternElement value)
        {
            switch (value)
            {
                case ChoicePatternElement _:
                    return ElementType.Choice;
                case OptionalPatternElement _:
                    return ElementType.Optional;
                case RegexMatchPatternElement _:
                    return ElementType.Regex;
                case TypePatternElement _:
                    return ElementType.Type;
                default:
                    return ElementType.None;
            }
        }
    }
}