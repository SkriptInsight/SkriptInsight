using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace SkriptInsight.Core.Annotations.Parameters
{
    [MeansImplicitUse(ImplicitUseTargetFlags.Itself | ImplicitUseTargetFlags.WithMembers)]
    [AttributeUsage(AttributeTargets.Property)]
    sealed class ParameterAttribute : Attribute
    {
        public int LineNumber { get; }

        public ParameterAttribute([CallerLineNumber] int lineNumber = 0)
        {
            LineNumber = lineNumber;
        }
    }
}