using System;
using SkriptInsight.Core.Files.Processes;

namespace SkriptInsight.Core.Files.Nullability
{
    public class MetadataSkriptFile : SkriptFile
    {
        public MetadataSkriptFile(Uri url) : base(url)
        {
        }

        protected override FileProcess ProvideParseProcess()
        {
            return null;
        }
    }
}