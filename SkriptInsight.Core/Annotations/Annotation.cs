using System;
using JetBrains.Annotations;

namespace SkriptInsight.SkriptDoc.Annotations
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public abstract class Annotation : Attribute
    {
    }
}