using System;
using JetBrains.Annotations;

namespace SkriptInsight.SkriptDoc.Annotations
{
    [MeansImplicitUse(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.Itself | ImplicitUseTargetFlags.WithMembers)]
    [AttributeUsage(AttributeTargets.Class)]
    sealed class AliasAttribute : Attribute
    {
        public string[] Aliases { get; }

        public AliasAttribute(params string[] aliases)
        {
            Aliases = aliases;
        }
    }
}