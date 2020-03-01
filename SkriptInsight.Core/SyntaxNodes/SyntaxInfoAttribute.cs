using System;
using JetBrains.Annotations;

namespace SkriptInsight.Core.SyntaxNodes
{
    [MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    sealed class SyntaxInfoAttribute : Attribute
    {
        public string ClassName { get; }

        public SyntaxInfoAttribute(string className)
        {
            ClassName = className;
        }
    }
}