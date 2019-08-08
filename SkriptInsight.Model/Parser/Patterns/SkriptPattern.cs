using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using SkriptInsight.Model.Parser.Patterns.Impl;

namespace SkriptInsight.Model.Parser.Patterns
{
    public class SkriptPattern : AbstractSkriptPatternElement
    {
        private static readonly Type[] GroupTypes =
        {
            typeof(ChoicePatternElement),
            typeof(OptionalPatternElement),
            typeof(TypePatternElement),
            typeof(RegexMatchPatternElement)
        };

        public bool FastFail { get; set; }
        
        public static SkriptPattern ParsePattern(ParseContext ctx)
        {
            var pattern = new SkriptPattern();

            foreach (var c in ctx)
            {
                var (type, info) = GroupTypes
                    .Select(cc => (Type: cc, Info: cc.GetCustomAttribute<GroupPatternElementInfoAttribute>()))
                    .FirstOrDefault(cc => cc.Item2.OpeningBracket == c);

                if (info != null)
                {
                    var closingBracketPos =
                        ctx.FindNextBracket(info.OpeningBracket, info.ClosingBracket);

                    if (closingBracketPos >= 0)
                    {
                        var innerText = ctx.ReadUntilPosition(closingBracketPos);

                        //Read closing bracket
                        ctx.ReadNext(1);

                        pattern.Children.Add(
                            Activator.CreateInstance(type, innerText) as AbstractGroupPatternElement);
                    }
                }
                else
                {
                    pattern.Children.Add(new LiteralPatternElement(c.ToString()));
                }
            }

            return pattern;
        }

        public List<AbstractSkriptPatternElement> Children { get; set; } = new List<AbstractSkriptPatternElement>();

        public override ParseResult Parse(ParseContext ctx)
        {
            var shouldFastFail = false;
            var results = Children.WithContext().Select(c =>
            {
                var oldPos = ctx.CurrentPosition;
                var ourPattern = this;
                ctx.ElementContext = c;
                if (shouldFastFail && FastFail) return ParseResult.Failure(ctx);
                
                var parse = c.Current.Parse(ctx);
                if (!parse.IsSuccess) ctx.CurrentPosition = oldPos;
                shouldFastFail |= !parse.IsSuccess; 
                
                return parse;
            }).ToList();
            
            if (!results.All(c => c.IsSuccess)) return ParseResult.Failure(ctx);
            
            var finalResult = ParseResult.Success(ctx);
            finalResult.ParseMark = results.Select(c => c.ParseMark).Aggregate(0, (left, right) => left ^ right);
            return finalResult;
        }

        public override string RenderPattern()
        {
            return string.Join("", Children.Select(c => c.RenderPattern()));
        }
    }
}