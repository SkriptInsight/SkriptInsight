namespace SkriptInsight.Core.Files.Nodes
{
    public abstract class SignatureFileNode<T> : AbstractFileNode
    {
        public T Signature { get; set; }
    }
}