using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using SkriptInsight.Core.Annotations.Parameters;

namespace SkriptInsight.Core.Annotations
{
    public class AnnotationDescription
    {
        public AnnotationDescription(AnnotationParser parser, Type type, Func<Annotation> initializer)
        {
            Parameters = type.GetProperties()
                .Select(c => (prop: c, param: c.GetCustomAttribute<ParameterAttribute>()))
                .Where(c => c.param != null) // Get all properties with parameters
                .OrderBy(c => c.param.LineNumber) // Sort by line number so params are correctly ordered
                .Select(c => new ParameterDescription(parser, c.prop)) // Create description of parameter
                .ToArray();
            Initializer = initializer;
        }

        public Func<Annotation> Initializer { get; set; }

        public ParameterDescription[] Parameters { get; set; }
    }
}