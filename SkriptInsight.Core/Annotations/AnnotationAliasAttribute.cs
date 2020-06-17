using System;
using JetBrains.Annotations;

namespace SkriptInsight.Core.Annotations
{
    [MeansImplicitUse(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature,
        ImplicitUseTargetFlags.Itself | ImplicitUseTargetFlags.WithMembers)]
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AnnotationAliasAttribute : Attribute
    {
        public string[] Aliases { get; }

        public AnnotationAliasAttribute(params string[] aliases)
        {
            Aliases = aliases;
        }
    }
}