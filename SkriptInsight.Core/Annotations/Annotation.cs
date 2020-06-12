using System;

namespace SkriptInsight.Core.Annotations
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public abstract class Annotation : Attribute
    {
    }
}