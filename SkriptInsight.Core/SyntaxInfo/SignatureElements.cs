namespace SkriptInsight.Core.SyntaxInfo
{
    internal static class SignatureElements
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
    }
}