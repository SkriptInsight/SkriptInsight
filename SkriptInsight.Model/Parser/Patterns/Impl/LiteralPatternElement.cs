using System;

namespace SkriptInsight.Model.Parser.Patterns.Impl
{
    public class LiteralPatternElement : AbstractSkriptPatternElement
    {
        public LiteralPatternElement(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        public override ParseResult Parse(ParseContext ctx)
        {
            var shouldSkipWhitespaces =
                string.IsNullOrWhiteSpace(ctx.PeekPrevious(1)) &&
                string.IsNullOrWhiteSpace(Value);

            if (shouldSkipWhitespaces)
                return ParseResult.OptionalSuccess(ctx);
                
            return ctx.ReadNext(Value.Length).EqualsIgnoreCase(Value) ? ParseResult.Success(ctx)
                : ParseResult.Failure(ctx);
        }

        public override string RenderPattern()
        {
            return Value;
        }

        public static explicit operator LiteralPatternElement(string str)
        {
            return new LiteralPatternElement(str);
        }
    }
}