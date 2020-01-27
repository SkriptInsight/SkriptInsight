using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Apache.NBCEL;
using Apache.NBCEL.ClassFile;
using Apache.NBCEL.Util;
using SkriptInsight.JavaMetadata;
using SkriptInsight.JavaReader;

namespace SkriptInsight.JavaMetadataExtractorLib
{
    public class MetadataExtractor
    {
        public static bool ReadJreStandardFile(out JarArchive archive)
        {
            archive = null;
            var javaHome = Environment.GetEnvironmentVariable("JAVA_HOME");
            if (javaHome == null) return false;
            if (Path.GetDirectoryName(javaHome).Contains("jdk"))
            {
                //Found jdk directory. Browse to the jre that's built in.
                javaHome = Path.Combine(javaHome, "jre");
            }
            if (!Directory.Exists(javaHome)) return false;

            var filePath = Path.Combine(javaHome, "lib", "rt.jar");
            if (!File.Exists(filePath)) return false;
            
            archive = ReadFile(filePath);
            return true;
        }
        
        public static JarArchive ReadFile(string path)
        {
            return new JarArchive(path);
        }
    }
}