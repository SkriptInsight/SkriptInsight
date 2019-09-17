using System.Linq;
using Newtonsoft.Json;
using SkriptInsight.Core.Parser.Patterns;

namespace SkriptInsight.Core.SyntaxInfo
{
    public abstract class AbstractSyntaxElement
    {
        public string[] Patterns { get; set; }

        public string AddonName { get; set; }

        [JsonIgnore] public SkriptPattern[] PatternNodes { get; set; }

        public void LoadPatterns()
        {
            if (Patterns != null)
                PatternNodes = Patterns.Select(c => SkriptPattern.ParsePattern(c)).ToArray();
            else
                PatternNodes = new SkriptPattern[0];
        }
    }
}