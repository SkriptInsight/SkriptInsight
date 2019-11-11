using System;
using JetBrains.Annotations;

namespace SkriptInsight.Core.Parser.Types
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    [MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
    internal sealed class TypeDescriptionAttribute : Attribute
    {
        public string TypeName { get; }

        public TypeDescriptionAttribute(string typeName)
        {
            TypeName = typeName;
        }
    }
}