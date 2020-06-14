using System;
using JetBrains.Annotations;

namespace SkriptInsight.Core.Files.Nodes.Impl.Signatures.ControlFlow
{
    [MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    sealed class ConditionalBasePrefixAttribute : Attribute
    {
        public string Prefix { get; }
        public bool IsOptional { get; }

        public ConditionalBasePrefixAttribute(string prefix, bool isOptional = false)
        {
            Prefix = prefix;
            IsOptional = isOptional;
        }
    }
}