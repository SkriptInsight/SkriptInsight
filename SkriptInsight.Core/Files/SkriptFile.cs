using System;

namespace SkriptInsight.Core.Files
{
    public class SkriptFile
    {
        public SkriptFile(Uri url)
        {
            Url = url;
            ParseContext = new FileParseContext(this);
        }

        public Uri Url { get; set; }

        public string[] RawContents { get; set; } = { };

        public FileParseContext ParseContext { get; }

    }
}