using System.Collections.Generic;

namespace SkriptInsight.Core.Annotations.Parameters
{
    public abstract class GenericParameterReader<T> : IParameterReader
    {
        protected abstract T GenericTryParse(Stack<string> args);

        public object? TryParse(Stack<string> args)
        {
            return GenericTryParse(args);
        }
    }
}