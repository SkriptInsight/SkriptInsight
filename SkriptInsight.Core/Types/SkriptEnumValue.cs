using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Parser;
using SkriptInsight.Core.Parser.Patterns;
using SkriptInsight.Core.Parser.Patterns.Impl;

namespace SkriptInsight.Core.Types
{
    public class SkriptEnumValue<T> where T: Enum
    {
        private T _value;
        
        private static Dictionary<T, SkriptPattern> ValuesMap { get; }

        static SkriptEnumValue()
        {
            ValuesMap = Enum.GetValues(typeof(T)).Cast<T>()
                .Select(c => (c,
                    new SkriptPattern
                    {
                        Children =
                        {
                            new ChoicePatternElement
                            {
                                Elements = c.GetAliases().Concat(new []{c.ToString().ToLower()}).Select(a =>
                                    new ChoicePatternElement.ChoiceGroupElement(new LiteralPatternElement(a))).ToList()
                            }
                        }
                    }))
                .ToDictionary(c => c.Item1, c => c.Item2);
        }

        [CanBeNull]
        public static SkriptEnumValue<T> TryParse(ParseContext ctx)
        {
            var clone = ctx.Clone();
            foreach (var (color, skriptPattern) in ValuesMap)
            {
                var result = skriptPattern.Parse(clone);
                if (!result.IsSuccess) continue;
                
                ctx.ReadUntilPosition(clone.CurrentPosition);
                return new SkriptEnumValue<T>(color, result.Matches.Last().RawContent);
            }

            return null;
        }

        public SkriptEnumValue(T value, string usedAlias)
        {
            _value = value;
            UsedAlias = usedAlias;
        }

        public SkriptEnumValue(string alias, T def = default)
        {
            UsedAlias = alias;
            Value = Enum.GetValues(typeof(T)).Cast<T>()
                .Where(ex => ex.GetAliases().Contains(alias.ToLower())).DefaultIfEmpty(def)
                .FirstOrDefault();
        }


        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                if (string.IsNullOrEmpty(UsedAlias))
                    UsedAlias = value.GetAliases().First();
            }
        }

        public string UsedAlias { get; set; }
        
        public override string ToString()
        {
            return UsedAlias;
        }
    }
}