using SkriptInsight.Core.Types.Attributes;

namespace SkriptInsight.Core.Types
{
    public enum WeatherType
    {
        [PatternAlias("sunny", "clear", "sun")]
        Clear,

        [PatternAlias("raining", "rainy", "rain")]
        Rain,

        [PatternAlias("thunderstorm", "thundering", "thunder")]
        Thunder
    }
}