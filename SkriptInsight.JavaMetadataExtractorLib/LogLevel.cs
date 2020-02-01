using System;

namespace SkriptInsight.JavaMetadataExtractorLib
{
    public enum LogLevel
    {
        None,
        Normal,
        Verbose,
        Debug
    }

    public static class LogLevelExtensions
    {
        public static bool IsSameOrHigher(this LogLevel value, LogLevel flag)
        {
            return value >= flag;
        }
    }
}