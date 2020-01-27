using System;
using System.IO;
using System.Linq;
using System.Text;
using Apache.NBCEL.Java.IO;
using PeterO.Cbor;
using SkriptInsight.JavaMetadata.Model;
using SkriptInsight.JavaMetadataExtractorLib.Serialization;

namespace SkriptInsight.JavaMetadataExtractorLib
{
    public static class MetadataIo
    {
        public static readonly byte[] FileHeader = Encoding.UTF8.GetBytes("SkriptInsight Rocks!");
        
        public static void WriteArchiveMetadata(string path, MetadataJarArchive archive)
        {
            using var fileStream = File.OpenWrite(path);
            fileStream.Write(FileHeader);
            
            using var dos = new DataOutputStream(new SharpOutputStream(fileStream));
            dos.WriteJarArchive(archive);
        }
        
        public static MetadataJarArchive ReadArchiveMetadata(string path)
        {
            using var fileStream = File.OpenRead(path);
            var header = new byte[FileHeader.Length];
            fileStream.Read(header);
            if (!header.SequenceEqual(FileHeader))
                return null;
            
            using var dis = new DataInputStream(new SharpInputStream(fileStream));
            var result = dis.ReadJarArchive();

            return result;
        }
    }
}