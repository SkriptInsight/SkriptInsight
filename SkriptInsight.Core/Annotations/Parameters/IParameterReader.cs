using System.Collections.Generic;

namespace SkriptInsight.SkriptDoc.Annotations.Parameters
{
    public interface IParameterReader
    {
        object? TryParse(Stack<string> args);
    }
}