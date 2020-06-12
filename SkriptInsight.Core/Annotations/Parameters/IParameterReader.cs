using System.Collections.Generic;

namespace SkriptInsight.Core.Annotations.Parameters
{
    public interface IParameterReader
    {
        object? TryParse(Stack<string> args);
    }
}