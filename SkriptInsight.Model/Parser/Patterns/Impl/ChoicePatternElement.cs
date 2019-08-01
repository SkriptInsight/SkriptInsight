using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace SkriptInsight.Model.Parser.Patterns.Impl
{
    [DebuggerDisplay("{" + nameof(RenderPattern) + "()}")]
    [GroupPatternElementInfo('(', ')')]
    public class ChoicePatternElement : AbstractGroupPatternElement
    {
        [DebuggerDisplay("{ParseMark}¦{Element.RenderPattern()}")]
        public class ChoiceGroupElement
        {
            public static ChoiceGroupElement FromElement(string code)
            {
                var match = code;
                var split = match.Split("¦");

                int.TryParse(split.Length > 1 ? split.FirstOrDefault() : "0", out var parseMark);
                if (split.Length > 1)
                {
                    match = split.Skip(1).FirstOrDefault();
                }

                return new ChoiceGroupElement(SkriptPattern.ParsePattern(ParseContext.FromCode(match)), parseMark);
            }

            public ChoiceGroupElement(AbstractSkriptPatternElement element, int mark = 0)
            {
                Element = element;
                ParseMark = mark;
            }

            public AbstractSkriptPatternElement Element { get; }

            public int ParseMark { get; }

            public override string ToString() => $"{(ParseMark > 0 ? $"{ParseMark}¦" : "")}{Element.RenderPattern()}";

            public static implicit operator ChoiceGroupElement(AbstractSkriptPatternElement e)
            {
                return new ChoiceGroupElement(e);
            }
        }

        public ChoicePatternElement()
        {
        }

        public ChoicePatternElement(string contents) : base(contents)
        {
            Elements = Regex.Split(contents, @"\|(?![^(]*\))")
                .Select(ChoiceGroupElement.FromElement)
                .ToList();
        }

        public List<ChoiceGroupElement> Elements { get; } = new List<ChoiceGroupElement>();

        public bool SaveChoice { get; set; } = true;
        
        public override ParseResult Parse(ParseContext ctx)
        {
            var oldPos = ctx.CurrentPosition;

            var matchedChoice = Elements.FirstOrDefault(e =>
            {
                ctx.CurrentPosition = oldPos;
                ctx.StartMatch();
                var result = e.Element.Parse(ctx);

                if (result?.IsSuccess ?? false)
                    ctx.EndMatch(SaveChoice);
                else
                    ctx.UndoMatch();

                return result?.IsSuccess ?? false;
            });
            if (matchedChoice == null) return ParseResult.Failure(ctx);

            var success = ParseResult.Success(ctx);
            success.ParseMark ^= matchedChoice.ParseMark;
            return success;
        }

        public override string RenderPattern()
        {
            return $"({string.Join("|", Elements)})";
        }
    }
}