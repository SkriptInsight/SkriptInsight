namespace SkriptInsight.Core
{
    /// <summary>
    /// Represents an element that knows the element that's previous and next to the current one
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
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