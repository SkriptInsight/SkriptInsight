using System;
using System.Collections.Generic;
using System.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using SkriptInsight.Core.Parser.Patterns;
using SkriptInsight.Core.Parser.Patterns.Impl;
using SkriptInsight.Core.Types;

namespace SkriptInsight.Core.Parser.Types.Impl
{
    [TypeDescription("color")]
    public class SkriptColor : SkriptGenericType<SkriptColor.SkriptRepresentation>
    {
        private Dictionary<ChatColor, SkriptPattern> ColorMap { get; }

        public SkriptColor()
        {
            ColorMap = Enum.GetValues(typeof(ChatColor)).Cast<ChatColor>()
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

        protected override SkriptRepresentation TryParse(ParseContext ctx)
        {
            var clone = ctx.Clone();
            
            foreach (var (color, skriptPattern) in ColorMap)
            {
                var result = skriptPattern.Parse(clone);
                if (!result.IsSuccess) continue;
                
                ctx.ReadUntilPosition(clone.CurrentPosition);
                return new SkriptRepresentation(color, result.Matches.Last().RawContent);
            }

            return null;
        }

        public override string AsString(SkriptRepresentation obj)
        {
            return obj.UsedAlias;
        }

        public class SkriptRepresentation
        {
            private ChatColor _color;

            public SkriptRepresentation(ChatColor color, string usedAlias)
            {
                _color = color;
                UsedAlias = usedAlias;
            }

            public SkriptRepresentation(string alias)
            {
                UsedAlias = alias;
                Color = Enum.GetValues(typeof(ChatColor)).Cast<ChatColor>()
                    .Where(ex => ex.GetAliases().Contains(alias.ToLower())).DefaultIfEmpty(ChatColor.Reset)
                    .FirstOrDefault();
            }


            public ChatColor Color
            {
                get => _color;
                set
                {
                    _color = value;
                    if (string.IsNullOrEmpty(UsedAlias))
                        UsedAlias = value.GetAliases().First();
                }
            }

            public string UsedAlias { get; set; }
        }
    }
}