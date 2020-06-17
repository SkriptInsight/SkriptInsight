namespace SkriptInsight.Core.Annotations
{
    class UnknownAnnotation : Annotation
    {
        public UnknownAnnotation(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}