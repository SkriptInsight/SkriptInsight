using System;

namespace SkriptInsight.Core.Parser.Types
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal sealed class TypeDescriptionAttribute : Attribute
    {
        public string TypeName { get; }

        public TypeDescriptionAttribute(string typeName)
        {
            TypeName = typeName;
        }
    }
}