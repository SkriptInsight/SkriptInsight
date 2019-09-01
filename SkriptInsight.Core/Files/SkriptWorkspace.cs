using System.Collections.Generic;

namespace SkriptInsight.Core.Files
{
    /// <summary>
    /// Represents a Skript workspace
    /// </summary>
    public class SkriptWorkspace
    {
        public List<SkriptFile> Files { get; set; } = new List<SkriptFile>();
    }
}