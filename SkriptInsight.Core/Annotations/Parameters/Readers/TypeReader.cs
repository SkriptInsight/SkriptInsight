using System;
using System.Collections.Generic;
using System.Linq;

namespace SkriptInsight.Core.Annotations.Parameters.Readers
{
    public class TypeReader : GenericParameterReader<Type>
    {
        protected override Type GenericTryParse(Stack<string> args, ParameterContext context)
        {
            var className = args.Peek();
            var type = AppDomain.CurrentDomain.GetAssemblies().SelectMany(c => c.GetTypes())
                .FirstOrDefault(c => c.Name == className || c.FullName == className);

            if (type != null)
                args.Pop();

            return type;
        }
    }
}