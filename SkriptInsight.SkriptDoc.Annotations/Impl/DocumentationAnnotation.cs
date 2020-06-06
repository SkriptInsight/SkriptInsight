using System.ComponentModel;
using SkriptInsight.SkriptDoc.Annotations.Parameters;

namespace SkriptInsight.SkriptDoc.Annotations.Impl
{
    [Alias("doc")]
    [Description("Annotation used to document this element")]
    public class DocumentationAnnotation : Annotation
    {
        [Description("The description of this element")]
        [Parameter] public string Description { get; set; } = "";
    }
}