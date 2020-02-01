using System;
using JetBrains.Annotations;

namespace SkriptInsight.Core.Inspections
{
    [UsedImplicitly]
    [MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class InspectionDescriptionAttribute : Attribute
    {
        public InspectionDescriptionAttribute()
        {
        }
    }
}