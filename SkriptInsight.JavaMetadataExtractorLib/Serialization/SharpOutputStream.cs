using System.IO;
using Apache.NBCEL.Java.IO;

namespace SkriptInsight.JavaMetadataExtractorLib.Serialization
{
    public class SharpOutputStream : OutputStream
    {
        public Stream Stream { get; }

        public SharpOutputStream(Stream stream)
        {
            Stream = stream;
        }
        
        public override void Close()
        {
            Stream.Dispose();
        }

        public override void Write(int b)
        {
            Stream.WriteByte((byte) b);            
        }
    }
}