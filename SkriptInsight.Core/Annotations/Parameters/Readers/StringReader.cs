using System.Collections.Generic;

namespace SkriptInsight.Core.Annotations.Parameters.Readers
{
    public class StringReader : GenericParameterReader<string>
    {
        protected override string GenericTryParse(Stack<string> args, ParameterContext context)
        {
            return context.IsLastParameter ? string.Join("", args) : args.Pop();
        }
    }
}