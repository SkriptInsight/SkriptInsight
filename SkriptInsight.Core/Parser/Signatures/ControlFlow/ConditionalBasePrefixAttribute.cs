using System;

namespace SkriptInsight.Core.Parser.Signatures.ControlFlow
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    sealed class ConditionalBasePrefixAttribute : Attribute
    {
        public string Prefix { get; }

        public ConditionalBasePrefixAttribute(string prefix)
        {
            Prefix = prefix;
        }
    }
}