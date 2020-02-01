namespace SkriptInsight.JavaMetadata.Model
{
    public class MetadataJavaClass : ObjectWithAccessFlags
    {
        public string ClassName { get; set; }
        
        public string[] AllSuperClasses { get; set; }

        public string[] Interfaces { get; set; }
        
        public string[] AllInterfaces { get; set; }
        
        public MetadataJavaMethod[] Methods { get; set; }
        
        public MetadataJavaField[] Fields { get; set; }
    }
}