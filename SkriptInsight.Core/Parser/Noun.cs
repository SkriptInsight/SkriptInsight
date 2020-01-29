using SkriptInsight.Core.Parser.Patterns;

namespace SkriptInsight.Core.Parser
{
    /// <summary>
    /// Represents a Skript noun
    /// </summary>
    public class Noun
    {
        public string Gender { get; set; }

        public string Singular { get; set; }

        public string Plural { get; set; }

        public SkriptPattern Pattern { get; set; }
    }
}