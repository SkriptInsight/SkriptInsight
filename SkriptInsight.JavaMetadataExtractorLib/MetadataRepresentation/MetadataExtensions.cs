using System;
using System.Linq;
using SkriptInsight.JavaMetadata.Model;
using SkriptInsight.JavaReader;

namespace SkriptInsight.JavaMetadataExtractorLib.MetadataRepresentation
{
    public static class MetadataExtensions
    {
        public static DataJavaField ToDataClass(this MetadataJavaField field)
        {
            return new DataJavaField(field);
        }
        
        public static DataJavaMethodParameter ToDataClass(this MetadataJavaMethodParameter meta)
        {
            return new DataJavaMethodParameter(meta);
        }
        
        public static DataJavaMethod ToDataClass(this MetadataJavaMethod meta)
        {
            return new DataJavaMethod(meta);
        }
        
        public static DataJavaClass ToDataClass(this MetadataJavaClass meta)
        {
            return new DataJavaClass(meta);
        }
        
        public static JarArchive ToDataClass(this MetadataJarArchive meta)
        {
            var archive = new JarArchive("Memory-File.jar", new byte[0]);
            var javaClasses = meta.JavaClasses.Select(c => (c.Key, c.Value.ToDataClass()));
            foreach (var (name, clazz) in javaClasses)
            {
                LoadedClassRepository.Instance.StoreClass(clazz);
                archive.Classes.Add(name, clazz);
            }

            return archive;
        }

        public static void LoadDataProperties(this JarArchive archive)
        {
            GC.Collect();
            foreach (var (_, @class) in archive.Classes)
            {
                if (@class is DataJavaClass dataJavaClass)
                    dataJavaClass.OnCanLoad();
            }
        }
    }
}