using JetBrains.Annotations;
using SkriptInsight.Model.Parser.Patterns;
using SkriptInsight.Model.Parser.Patterns.Impl;

namespace SkriptInsight.Model.Parser.Types.Impl
{
    [TypeDescription("boolean")]
    public class SkriptBoolean : SkriptType<bool>
    {
        public ChoicePatternElement BooleanPattern { get; set; }

        public SkriptBoolean()
        {
            //(1¦on|2¦off|3¦true|4¦false)
            BooleanPattern = new ChoicePatternElement
            {
                Elements =
                {
                    new ChoicePatternElement.ChoiceGroupElement(new LiteralPatternElement("on"), 1),
                    new ChoicePatternElement.ChoiceGroupElement(new LiteralPatternElement("off"), 2),
                    new ChoicePatternElement.ChoiceGroupElement(new LiteralPatternElement("true"), 3),
                    new ChoicePatternElement.ChoiceGroupElement(new LiteralPatternElement("false"), 4)
                }
            };
        }
        
        protected override Expression<bool> ParseExpression(ParseContext ctx, SyntaxValueAcceptanceConstraint constraint)
        {
            var result = BooleanPattern.Parse(ctx);
            if (result.IsSuccess)
            {
                var value = result.ParseMark % 2 != 0; //Parse mark is odd when it's a false value
            }

            return null;
        }

        protected override string RenderExpression(Expression<bool> value)
        {
            throw new System.NotImplementedException();
        }
    }
}