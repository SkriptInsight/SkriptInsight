using JetBrains.Annotations;

namespace SkriptInsight.Core.Files.Nodes {
    [MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
    public abstract class SignatureFileNode<T> : AbstractFileNode
    {
        protected SignatureFileNode(T signature)
        {
            Signature = signature;
        }

        public T Signature { get; set; }

        public override string ToString()
        {
            return Signature?.ToString() ?? "";
        }
    }
}