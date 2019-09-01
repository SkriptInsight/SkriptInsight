using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using SkriptInsight.Core.Parser.Patterns;

namespace SkriptInsight.Core.SyntaxInfo
{
    [DebuggerDisplay("{" + nameof(ClassName) + "}")]
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