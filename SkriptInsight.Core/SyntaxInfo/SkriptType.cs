using System.Linq;
using System.Text.RegularExpressions;
using SkriptInsight.Core.Parser;
using SkriptInsight.Core.Parser.Patterns;

namespace SkriptInsight.Core.SyntaxInfo
{
    public class SkriptType
    {
        public int Id { get; set; }

        public string[] Description { get; set; }

        public string[] Usage { get; set; }

        public string[] Examples { get; set; }

        public string Since { get; set; }

        public string TypeName { get; set; }

        public string AddonName { get; set; }

        public string ClassName { get; set; }

        public string[] PossibleValues { get; set; }

        public SkriptPattern[] PossibleValuesPatterns { get; set; }

        public string[] Patterns { get; set; }

        public Regex[] PatternsRegexes { get; set; }

        public void LoadPatterns()
        {
            PossibleValuesPatterns = PossibleValues?.Select(SkriptNounParser.ConvertNounToPattern).ToArray();
            PatternsRegexes = Patterns?
                .Select(c => new Regex('^' + c + '$', RegexOptions.Compiled | RegexOptions.IgnoreCase))
                .ToArray();
        }
    }
}