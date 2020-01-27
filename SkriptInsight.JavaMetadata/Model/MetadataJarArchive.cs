using System.Collections.Generic;

namespace SkriptInsight.JavaMetadata.Model
{
    public class MetadataJarArchive
    {
        public Dictionary<string, MetadataJavaClass> JavaClasses { get; set; } =
            new Dictionary<string, MetadataJavaClass>();
    }
}