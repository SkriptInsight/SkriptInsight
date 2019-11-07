using JetBrains.Annotations;

namespace SkriptInsight.Core.Files.Nodes
{
    [MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
    public abstract class SignatureFileNode<T> : AbstractFileNode
    {
        public T Signature { get; set; }

        public override string ToString()
        {
            return Signature?.ToString() ?? "";
        }
    }
}