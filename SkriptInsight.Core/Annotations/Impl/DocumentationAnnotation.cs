using System.ComponentModel;
using SkriptInsight.Core.Annotations.Parameters;

namespace SkriptInsight.Core.Annotations.Impl
{
    [Alias("doc")]
    [Description("Annotation used to document this element")]
    public class DocumentationAnnotation : Annotation
    {
        [Description("The description of this element")]
        [Parameter]
        public string Description { get; set; } = "";
    }
}