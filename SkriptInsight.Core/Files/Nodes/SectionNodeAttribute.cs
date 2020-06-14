using System;

namespace SkriptInsight.Core.Files.Nodes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    sealed class SectionNodeAttribute : Attribute
    {
        public bool Optional { get; }

        public SectionNodeAttribute(bool optional = false)
        {
            Optional = optional;
        }
    }
}