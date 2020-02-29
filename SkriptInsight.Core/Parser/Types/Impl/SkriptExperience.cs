using System;
using System.Collections.Generic;
using Humanizer;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Parser.Expressions;
using SkriptInsight.Core.Parser.Patterns;
using SkriptInsight.Core.Parser.Patterns.Impl;
using SkriptInsight.Core.Types;

namespace SkriptInsight.Core.Parser.Types.Impl
{
    [TypeDescription("experience")]
    public class SkriptExperience : SkriptGenericType<Experience>
    {
        static SkriptExperience()
        {
            ParsePattern = new SkriptPattern
            {
                Children =
                {
                    new OptionalPatternElement
                    {
                        Element = new SkriptPattern
                        {
                            Children =
                            {
                                new TypePatternElement
                                {
                                    Type = "number"
                                }
                            }
                        }
                    },
                    new LiteralPatternElement(" "),
                    new ChoicePatternElement
                    {
                        Elements =
                        {
                            new ChoicePatternElement.ChoiceGroupElement(new SkriptPattern
                            {
                                Children =
                                {
                                    new LiteralPatternElement("experience"),
                                    new OptionalPatternElement
                                    {
                                        Element = new SkriptPattern
                                        {
                                            Children =
                                            {
                                                new LiteralPatternElement(" "),
                                                new ChoicePatternElement
                                                {
                                                    Elements =
                                                    {
                                                        new ChoicePatternElement.ChoiceGroupElement(
                                                            new SkriptPattern
                                                            {
                                                                Children =
                                                                {
                                                                    new LiteralPatternElement("point"),
                                                                    new OptionalPatternElement {Element = new LiteralPatternElement("s")}
                                                                }
                                                            }, 5)
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }, 2),
                            new ChoicePatternElement.ChoiceGroupElement(new SkriptPattern
                            {
                                Children =
                                {
                                    new ChoicePatternElement
                                    {
                                        Elements =
                                        {
                                            new ChoicePatternElement.ChoiceGroupElement(
                                                new OptionalPatternElement {Element = new LiteralPatternElement("e")},
                                                4)
                                        }
                                    },
                                    new LiteralPatternElement("xp")
                                }
                            }, 1)
                        }
                    }
                }
            };
        }

        private static SkriptPattern ParsePattern { get; }


        protected override Experience TryParse(ParseContext ctx, List<MatchAnnotation> matchAnnotationsHolder)
        {
            var clone = ctx.Clone();

            var result = ParsePattern.Parse(clone);

            if (!result.IsSuccess) return null;

            var resultObj = new Experience();
            var number = result.Matches.GetExplicitValue<double?>(0);
            resultObj.Type = (ExperienceType) result.ParseMark;

            
            if (number?.GenericValue.HasValue == true)
                resultObj.Amount = number.GenericValue.Value;
            else
                matchAnnotationsHolder.Add(new MatchAnnotation(MatchAnnotationSeverity.Error,
                    "ExperienceNumberIsNotIncluded",
                    $"The amount of {resultObj.Type.Humanize()} must be included"));

            ctx.ReadUntilPosition(clone.CurrentPosition);
            return resultObj;
        }

        public override string AsString(Experience obj)
        {
            return obj.ToString();
        }
    }
}