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
        public static event EventHandler<(LogLevel level, string message)> LogMessage;
        
        public static readonly byte[] FileHeader = Encoding.UTF8.GetBytes("SkriptInsight Rocks!");
        
        public static void WriteArchiveMetadata(string path, MetadataJarArchive archive)
        {
            Log($"Opening file \"{path}\" to start metadata write.");
            using var fileStream = File.OpenWrite(path);
            LogVerbose($"Writing file header to \"{path}\".");
            fileStream.Write(FileHeader);
            
            LogDebug($"Started writing jar archive metadata to \"{path}\".");
            using var dos = new DataOutputStream(new SharpOutputStream(fileStream));
            dos.WriteJarArchive(archive);
            LogDebug($"Finished writing jar archive metadata to \"{path}\".");
            Log($"Finished writing jar metadata to \"{path}\".");
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

        public static void Log(LogLevel level, string e)
        {
            LogMessage?.Invoke(null, (level, e));
        }

        public static void Log(string e) => Log(LogLevel.Normal, e);
        public static void LogVerbose(string e) => Log(LogLevel.Verbose, e);
        
        public static void LogDebug(string e) => Log(LogLevel.Debug, e);
    }
}