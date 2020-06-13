using System;

namespace SkriptInsight.Core.Files.Nodes
{
    [AttributeUsage(AttributeTargets.Class )]
    sealed class SectionNodeAttribute : Attribute
    {
        public SectionNodeAttribute()
        {
        }
    }
}