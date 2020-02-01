using SkriptInsight.Core.Types.Attributes;

namespace SkriptInsight.Core.Types
{
    public enum GameMode
    {
        [PatternAlias("survival")]
        Survival,
        [PatternAlias("creative")]
        Creative,
        [PatternAlias("adventure")]
        Adventure,
        [PatternAlias("spectator")]
        Spectator
    }
}