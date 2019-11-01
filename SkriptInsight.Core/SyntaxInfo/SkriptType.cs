using System.Linq;
using System.Text.RegularExpressions;
using SkriptInsight.Core.Parser;
using SkriptInsight.Core.Parser.Expressions;
using SkriptInsight.Core.Parser.Patterns;
using SkriptInsight.Core.Parser.Types.Impl.Internal;

namespace SkriptInsight.Core.SyntaxInfo
{
    public class SkriptType
    {
        public static readonly SkriptType Void = new SkriptType
        {
            AddonName = "Skript",
            Id = 0,
            Since = "Skript 1.0",
            ClassName = "Void",
            TypeName = "void",
            Description = new[]
                {"Default void type. Used by SkriptInsight to denote the lack of a return type on functions"}
        };
        public static readonly Expression<SkriptType> VoidExpr = new Expression<SkriptType>(Void)
        {
            Type = new SkriptVoid()
        };


        public int Id { get; set; }

        public string[] Description { get; set; }

        public string[] Usage { get; set; }

        public string[] Examples { get; set; }

        public string Since { get; set; }

        public string TypeName { get; set; }

        public string AddonName { get; set; }

        public string ClassName { get; set; }

        public string[] PossibleValues { get; set; }

        public Noun[] PossibleValuesAsNouns { get; set; }

        public string[] Patterns { get; set; }

        public Regex[] PatternsRegexes { get; set; }

        public void LoadPatterns()
        {
            PossibleValuesAsNouns = PossibleValues?.Select(SkriptNounParser.ParseNoun).ToArray();
            PatternsRegexes = Patterns?
                .Select(c => new Regex('^' + c + '$', RegexOptions.Compiled | RegexOptions.IgnoreCase))
                .ToArray();
        }
    }
}