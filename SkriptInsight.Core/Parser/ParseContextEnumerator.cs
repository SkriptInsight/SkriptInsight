using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SkriptInsight.Core.Parser
{
    /// <summary>
    /// Enumerator for a parse context. Goes through all chars by reading one by one.
    /// </summary>
    public class ParseContextEnumerator : IEnumerator<char>
    {
        private ParseContext Context { get; set; }

        public ParseContextEnumerator(ParseContext context)
        {
            Context = context;
        }

        public bool MoveNext()
        {
            Current = Context.ReadNext(1).FirstOrDefault();
            return !Context.HasReachedEndOfEnumerator;
        }

        public void Reset()
        {
            Context.CurrentPosition = Context.IndentationChars;
        }

        public char Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }
    }
}