using System.Collections.Generic;
using System.Linq;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Parser.Patterns.Impl;

namespace SkriptInsight.Core.Parser.Patterns
{
    public class PatternHelper
    {
        internal static IList<AbstractSkriptPatternElement> Flatten(AbstractSkriptPatternElement element)
        {
            return element switch
            {
                SkriptPattern skriptPattern => skriptPattern.Children.SelectMany(Flatten).ToList(),
                _ => new List<AbstractSkriptPatternElement> {element}
            };
        }

        internal static IList<AbstractSkriptPatternElement> GetPossibleInputs(
            IList<AbstractSkriptPatternElement> elements, bool skipOptionals = false)
        {
            IList<AbstractSkriptPatternElement> possibilities = new List<AbstractSkriptPatternElement>();
            foreach (var element in elements)
            {
                if (element is RegexMatchPatternElement || element is LiteralPatternElement)
                {
                    if (element is LiteralPatternElement patternElement)
                    {
                        var text = patternElement.Value;
                        if (text.Length == 0 || text.IsEmpty() && elements.Count == 1)
                        {
                            return possibilities;
                        }

                        if (text.IsEmpty())
                        {
                            continue;
                        }
                    }

                    possibilities.Add(element);
                    return possibilities;
                }

                if (element is ChoicePatternElement choicePatternElement)
                {
                    foreach (var possibleInputs in choicePatternElement.Elements.Select(choice => GetPossibleInputs(Flatten(choice.Element))))
                    {
                        ((List<AbstractSkriptPatternElement>) possibilities).AddRange(possibleInputs);
                    }

                    return possibilities;
                }

                if (element is TypePatternElement)
                {
                    possibilities.Add(element);
                    return possibilities;
                }

                if (!skipOptionals && element is OptionalPatternElement optionalPatternElement)
                {
                    ((List<AbstractSkriptPatternElement>) possibilities).AddRange(
                        GetPossibleInputs(Flatten(optionalPatternElement.Element)));
                }
            }

            return possibilities;
        }
    }
}