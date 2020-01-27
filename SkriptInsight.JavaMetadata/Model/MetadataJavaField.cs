namespace SkriptInsight.JavaMetadata.Model
{
    public class MetadataJavaField : ObjectWithAccessFlags
    {
        public string Name { get; set; }
        public string Type { get; set; }
        
        public byte[] ConstantValue { get; set; }
    }
}