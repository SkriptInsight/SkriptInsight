using System.Linq;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Parser.Patterns;
using SkriptInsight.Core.Parser.Patterns.MatchInfo;

namespace SkriptInsight.Core.Parser
{
    
    /// <summary>
    /// Represents a parse match.
    /// </summary>
    public class ParseMatch
    {
        public ParseMatch()
        {
            
        }
        
        [JsonIgnore] public ParseContext Context { get; set; }

        public Range Range { get; set; }

        public string RawContent { get; set; }

        [CanBeNull]
        public MatchElementInfo ElementInfo { get; set; }
        
        public override string ToString()
        {
            return RawContent;
        }

        public static MatchElementInfo LoadElementInfo(AbstractSkriptPatternElement patternElement)
        {
            int index;
            var type = patternElement.ToElementType();
            switch (patternElement?.Parent)
            {
                case null:
                    return new MatchElementInfo {Index = -1, Type = ElementType.None};
                case SkriptPattern skP:
                    index = skP.Children.Where(c => patternElement.GetType().IsInstanceOfType(c)).ToList().IndexOf(patternElement);
                    break;
                default:
                    return LoadElementInfo(patternElement.Parent);
            }

            return new MatchElementInfo {Type = type, Index = index};         
        }
    }
}