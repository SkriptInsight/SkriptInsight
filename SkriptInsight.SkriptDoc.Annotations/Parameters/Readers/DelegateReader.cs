using System;
using System.Collections.Generic;

namespace SkriptInsight.SkriptDoc.Annotations.Parameters.Readers
{
    public class DelegateReader : IParameterReader
    {
        public DelegateReader(Func<Stack<string>, object?> reader)
        {
            Reader = reader;
        }

        private Func<Stack<string>, object?> Reader { get; }


        public object? TryParse(Stack<string> args)
        {
            return Reader(args);
        }
    }
}