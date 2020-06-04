using System.Text.RegularExpressions;
using SkriptInsight.Core.Extensions;

namespace SkriptInsight.Core.Parser.Patterns.Impl
{
    public class LiteralPatternElement : AbstractSkriptPatternElement
    {
        public LiteralPatternElement(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        public override ParseResult Parse(ParseContext contextToUse)
        {
            var shouldSkipWhitespaces =
                (string.IsNullOrWhiteSpace(contextToUse.PeekPrevious(1)) || contextToUse.HasFinishedLine) &&
                string.IsNullOrWhiteSpace(Value);

            if (shouldSkipWhitespaces)
                return ParseResult.OptionalSuccess(contextToUse);

            return contextToUse.ReadNext(Value.Length).EqualsIgnoreCase(Value)
                ? ParseResult.Success(contextToUse)
                : ParseResult.Failure(contextToUse);
        }

        
        private static readonly Regex LiteralRegexReplacement = new Regex("([\\(\\)\\[\\]])", RegexOptions.Compiled);
        public override string RenderPattern()
        {
            return LiteralRegexReplacement.Replace(Value, "\\$1");
        }

        public static explicit operator LiteralPatternElement(string str)
        {
            return new LiteralPatternElement(str);
        }
    }
}