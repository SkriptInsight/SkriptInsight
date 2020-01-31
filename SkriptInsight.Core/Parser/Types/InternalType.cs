using System;
using JetBrains.Annotations;

namespace SkriptInsight.Core.Parser.Types
{
    /// <summary>
    /// Denotes that a type is an internal SkriptInsight type. <p/>
    /// Used to warn about usage of internal types
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    [MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
    sealed class InternalTypeAttribute : Attribute
    {
    }
}