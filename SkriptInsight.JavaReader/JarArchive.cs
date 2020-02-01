using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Apache.NBCEL;
using Apache.NBCEL.ClassFile;
using Apache.NBCEL.Util;

namespace SkriptInsight.JavaReader
{
    public class JarArchive
    {
        public string Name { get; set; }

        public byte[] RawContents { get; set; }

        public Dictionary<string, JavaClass> Classes = new Dictionary<string, JavaClass>();

        public JarArchive(string path) : this(Path.GetFileName(path), File.ReadAllBytes(path))
        {
        }

        public JarArchive(string name, byte[] bytes)
        {
            Name = name;
            RawContents = bytes;
            if (bytes.Length > 0)
                ReadJar();
        }

        private void ReadJar()
        {
            using var ms = new MemoryStream(RawContents);
            var archive = new ZipArchive(ms);
            foreach (var javaClass in archive.Entries
                .Where(c => Path.GetExtension(c.Name).ToLower() == ".class")
                .Select(e =>
                    {
                        //Let NBCEL parse the class file
                        var reader = new ClassParser(e.Open().ReadFully(), e.Name);
                        var result = reader.Parse();
                        var ownJavaClass = new JavaClass(result);

                        //Store the class for future use from NBCEL.
                        SyntheticRepository.GetInstance().StoreClass(result);
                        LoadedClassRepository.Instance.StoreClass(ownJavaClass);

                        return ownJavaClass;
                    }
                ))
            {
                Classes.Add(javaClass.FullClassName, javaClass);
            }
        }
    }
}