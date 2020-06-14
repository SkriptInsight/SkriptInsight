using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SkriptInsight.Core.Annotations.Parameters.Readers
{
    public class DelegateReader : IParameterReader
    {
        public DelegateReader(Func<Stack<string>, ParameterContext, object> reader)
        {
            Reader = reader;
        }

        private Func<Stack<string>, ParameterContext, object> Reader { get; }

        public object TryParse(Stack<string> args, ParameterContext context)
        {
            return Reader(args, context);
        }
    }
}