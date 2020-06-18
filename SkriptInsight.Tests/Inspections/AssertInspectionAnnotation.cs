using SkriptInsight.Core.Annotations;
using SkriptInsight.Core.Annotations.Impl;
using SkriptInsight.Core.Annotations.Parameters;

namespace SkriptInsight.Tests.Inspections
{
    [AnnotationAlias("assertinspection")]
    public class AssertInspectionAnnotation : Annotation
    {
        [Parameter] public string InspectionType { get; set; }
    }
}