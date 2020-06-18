namespace SkriptInsight.JavaMetadata.Model
{
    public class MetadataJavaMethod : ObjectWithAccessFlags
    {
        public string Name { get; set; }

        public string ReturnType { get; set; }

        public MetadataJavaMethodParameter[] Parameters { get; set; }
    }
}