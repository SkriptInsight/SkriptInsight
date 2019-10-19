using SkriptInsight.Core.Types.Attributes;

namespace SkriptInsight.Core.Types
{
    public enum GameDifficulty
    {
        [PatternAlias("easy")]
        Easy,
        [PatternAlias("medium", "normal")]
        Normal,
        [PatternAlias("hard")]
        Hard,
        [PatternAlias("peaceful")]
        Peaceful,

    }
}