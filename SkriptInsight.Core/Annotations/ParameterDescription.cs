using System.Reflection;
using JetBrains.Annotations;
using SkriptInsight.Core.Annotations.Parameters;

namespace SkriptInsight.Core.Annotations
{
    public class ParameterDescription
    {
        public ParameterDescription(AnnotationParser parser, PropertyInfo property)
        {
            Property = property;
            ParameterReader = parser.GetReaderForType(property.PropertyType);
        }

        public PropertyInfo Property { get; }

        [CanBeNull] public IParameterReader ParameterReader { get; }
    }
}