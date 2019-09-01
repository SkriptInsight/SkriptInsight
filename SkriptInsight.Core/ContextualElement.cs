namespace SkriptInsight.Core
{
    public class ContextualElement<T>
    {
        public T Previous { get; }
        
        public T Next { get; }
        
        public T Current { get; }

        public ContextualElement(T current, T previous, T next)
        {
            Current = current;
            Previous = previous;
            Next = next;
        }
    }
}