using System.Linq;
using Newtonsoft.Json;
using SkriptInsight.Model.Parser.Patterns;

namespace SkriptInsight.Model.SyntaxInfo
{
    public class SkriptEffect : ISyntaxElement
    {
        public string ClassName { get; set; }

        [JsonIgnore] public SkriptPattern[] PatternNodes { get; set; }

        public string[] Patterns { get; set; }

        public string AddonName { get; set; }

        public void LoadPatterns()
        {
            PatternNodes = Patterns.Select(c => SkriptPattern.ParsePattern(c)).ToArray();
        }
    }
}