using System;
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

        public static bool IsIndefiniteArticle(string s)
        {
            s = s.ToLower().Trim();
            return s.Equals("the") || s.Equals("a") || s.Equals("an");
        }

        public string WithAmount(in double amount)
        {
            return $"{amount} {(Math.Abs(amount - 1) < float.Epsilon ? Singular : Plural)}";
        }
    }
}