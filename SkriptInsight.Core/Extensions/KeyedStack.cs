using System.Collections.Generic;

namespace SkriptInsight.Core.Extensions
{
    public class KeyedStack<TKey, TValue>
    {
        private Dictionary<TKey, Stack<TValue>> InnerDictionary { get; } = new Dictionary<TKey, Stack<TValue>>();

        public Stack<TValue> this[TKey key] => GetStackForKey(key);

        private Stack<TValue> GetStackForKey(TKey key)
        {
            if (!InnerDictionary.ContainsKey(key))
                InnerDictionary[key] = new Stack<TValue>();
            return InnerDictionary[key];
        }
    }
}