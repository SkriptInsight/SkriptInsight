using System.IO;
using Apache.NBCEL.Java.IO;

namespace SkriptInsight.JavaMetadataExtractorLib.Serialization
{
    public class SharpInputStream : InputStream
    {
        public Stream Stream { get; }

        public SharpInputStream(Stream stream)
        {
            Stream = stream;
        }
        
        public override void Close()
        {
            Stream.Dispose();
        }
        
        public override int Available()
        {
            return (int) (Stream.Length - Stream.Position);
        }

        public override void Reset()
        {
            Stream.Position = 0;
        }

        public override bool MarkSupported()
        {
            return false;
        }

        public override int Read()
        {
            return Stream.ReadByte();
        }
    }
}