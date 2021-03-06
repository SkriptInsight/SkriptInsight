using SkriptInsight.Core.Annotations.Parameters;
using SkriptInsight.Core.Parser.Expressions;
using SkriptInsight.Core.Parser.Types.Impl.Internal;

namespace SkriptInsight.Core.Annotations.Impl
{
    [AnnotationAlias("param")]
    public class FunctionParameterAnnotation : Annotation
    {
        public FunctionParameterAnnotation(string parameter, string description)
        {
            Parameter = parameter;
            Description = description;
        }

        [Parameter] public string Parameter { get; set; }

        [Parameter] public string Description { get; set; }
    }
}