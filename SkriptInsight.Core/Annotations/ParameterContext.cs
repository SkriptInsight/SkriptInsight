namespace SkriptInsight.Core.Annotations
{
    public class ParameterContext
    {
        public ParameterContext(int index, bool isLastParameter)
        {
            Index = index;
            IsLastParameter = isLastParameter;
        }

        public int Index { get; set; }

        public bool IsLastParameter { get; set; }

        public void Deconstruct(out int index, out bool isLastParameter)
        {
            index = Index;
            isLastParameter = IsLastParameter;
        }
    }
}