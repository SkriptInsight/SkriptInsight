using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using SkriptInsight.Core.Parser.Patterns;

namespace SkriptInsight.Core.SyntaxInfo
{
    [UsedImplicitly]
    public abstract class AbstractSyntaxElement
    {
        public static readonly AbstractSyntaxElement EmptyLine = new SkriptEvent
        {
            AddonName = "Skript",
            Since = "Skript 0.0.1-SNAPSHOT",
            Cancellable = false,
            Id = -3,
            RequiredPlugins = new[] {"Skript"},
            DocumentationId = "SkriptEmptyLine",
            Name = "EmptyLine",
            ClassNames = new[] {"EmptyLine"}
        };
        
        public static readonly AbstractSyntaxElement FunctionSignature = new SkriptEvent
        {
            AddonName = "Skript",
            Since = "Skript 2.2",
            Cancellable = false,
            Id = -2,
            RequiredPlugins = new[] {"Skript"},
            DocumentationId = "SkriptFunctionDeclaration",
            Name = "FunctionSignature",
            ClassNames = new[] {"Function"}
        };

        public string[] Patterns { get; set; }

        public string AddonName { get; set; }

        [JsonIgnore] public SkriptPattern[] PatternNodes { get; set; }

        public void LoadPatterns()
        {
            if (Patterns != null)
                PatternNodes = Patterns.Select(c => SkriptPattern.ParsePattern(c, true)).ToArray();
            else
                PatternNodes = new SkriptPattern[0];
        }
    }
}