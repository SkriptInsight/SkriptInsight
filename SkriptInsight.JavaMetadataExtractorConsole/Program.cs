using System;
using SkriptInsight.JavaMetadata;
using SkriptInsight.JavaMetadataExtractorLib;
using SkriptInsight.JavaMetadataExtractorLib.MetadataRepresentation;

namespace SkriptInsight.JavaMetadataExtractorConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // MetadataExtractor.ReadJreStandardFile(out var jreArchive);

            // MetadataIo.WriteArchiveMetadata("jre-rt.simeta", jreArchive.ToMetadata());

            var metaJre = MetadataIo.ReadArchiveMetadata("jre-rt.simeta");

            var dataJre = metaJre.ToDataClass();
            
            dataJre.LoadDataProperties();
            GC.Collect();

            Console.WriteLine("Finished");
            Console.ReadLine();
        }
    }
}