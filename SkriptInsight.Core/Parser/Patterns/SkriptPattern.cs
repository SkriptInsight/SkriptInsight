using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Parser.Patterns.Impl;

namespace SkriptInsight.Core.Parser.Patterns
{
    /// <summary>
    /// Represents an entire Skript pattern.
    /// </summary>
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

        public List<AbstractSkriptPatternElement> Children { get; set; } = new List<AbstractSkriptPatternElement>();

        public static SkriptPattern ParsePattern(ParseContext ctx, bool fastFail = false)
        {
            var pattern = new SkriptPattern();
            var literalBuilder = new StringBuilder();

            void AddLiteralIfExists()
            {
                if (string.IsNullOrEmpty(literalBuilder.ToString())) return;
                pattern.Children.Add(new LiteralPatternElement(literalBuilder.ToString()));
                literalBuilder.Clear();
            }

            var isNextCharEscaped = false;
            foreach (var c in ctx)
            {
                if (c == '\\' && !isNextCharEscaped)
                {
                    isNextCharEscaped = true;
                    continue;
                }
                
                var (type, info) = GroupTypes
                    .Select(cc => (Type: cc, Info: cc.GetCustomAttribute<GroupPatternElementInfoAttribute>()))
                    .FirstOrDefault(cc => cc.Item2.OpeningBracket == c);

                if (!isNextCharEscaped && info != null)
                {
                    AddLiteralIfExists();
                    var closingBracketPos =
                        ctx.FindNextBracket(info.OpeningBracket, info.ClosingBracket, escapeWithBackSlash: true);

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
                    var isWhiteSpace = char.IsWhiteSpace(c);
                    if (isWhiteSpace)
                    {
                        AddLiteralIfExists();
                        pattern.Children.Add(new LiteralPatternElement(c.ToString()));
                    }
                    else
                        literalBuilder.Append(c);
                }
                isNextCharEscaped = false;
            }
            AddLiteralIfExists();

            pattern.FastFail = fastFail;
            return pattern;
        }

        public override ParseResult Parse(ParseContext ctx)
        {
            var possibleInputs = PatternHelper.GetPossibleInputs(Children, true);
            var isLeftElementExpression = possibleInputs.Count > 0 && possibleInputs[0] is TypePatternElement;
            var contextToUse = ctx;
            var shouldFastFail = false;
            var index = 0;
            var results = Children.WithContext().Select(c =>
            {
                c.Current.ElementIndex = index++;
                c.Current.Parent = this;
                
                if (isLeftElementExpression && c.Current.NarrowContextIfPossible(ref contextToUse, out var parseResult)) return parseResult;

                //Store old position in case of a rollback needed.
                var oldPos = contextToUse.CurrentPosition;
                //Pass the current element context to the parse context
                contextToUse.ElementContext = c;

                //If a match fails and this pattern needs to perform a fastfail (optionals), return a failure immediately
                if (shouldFastFail && FastFail) return ParseResult.Failure(contextToUse);
                
                var parse = c.Current.Parse(contextToUse);
                if (!parse.IsSuccess) contextToUse.CurrentPosition = oldPos;
                shouldFastFail |= !parse.IsSuccess;

                RestoreFromNarrowedContext(ctx, contextToUse);
                
                return parse;
            }).ToList();

            contextToUse.ElementContext = null;
            if (!results.All(c => c.IsSuccess)) return ParseResult.Failure(contextToUse);

            var finalResult = ParseResult.Success(contextToUse);
            //Calculate Parse Marks for this final result
            finalResult.ParseMark = results.Select(c => c.IsOptionallyMatched ? 0 : c.ParseMark).Aggregate(0, (left, right) => left ^ right);
            
            return finalResult;
        }

        public override string RenderPattern()
        {
            return string.Join("", Children.Select(c => c.RenderPattern()));
        }
    }
}