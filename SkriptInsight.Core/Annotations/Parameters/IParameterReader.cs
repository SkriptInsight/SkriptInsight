using System.Collections.Generic;
using JetBrains.Annotations;

namespace SkriptInsight.Core.Annotations.Parameters
{
    public interface IParameterReader
    {
        [CanBeNull]
        object TryParse(Stack<string> args, ParameterContext context);
    }
}