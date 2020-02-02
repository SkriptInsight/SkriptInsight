using System;
using JetBrains.Annotations;

namespace SkriptInsight.Core.Inspections
{
    [UsedImplicitly]
    [MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class InspectionDescriptionAttribute : Attribute
    {
        public string InspectionId { get; }

        public InspectionDescriptionAttribute(string id)
        {
            InspectionId = id;
        }
    }
}