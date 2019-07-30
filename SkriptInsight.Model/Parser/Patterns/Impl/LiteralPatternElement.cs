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
            return ctx.ReadNext(Value.Length).EqualsIgnoreCase(Value)
                ? ParseResult.Success(ctx)
                : ParseResult.Failure(ctx);
        }

        public override string RenderPattern()
        {
            return Value;
        }
    }
}