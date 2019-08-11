namespace SkriptInsight.Model
{
    public class ContextualElement<T>
    {
        public T Previous { get; private set; }
        public T Next { get; private set; }
        public T Current { get; private set; }

        public ContextualElement(T current, T previous, T next)
        {
            Current = current;
            Previous = previous;
            Next = next;
        }
    }
}