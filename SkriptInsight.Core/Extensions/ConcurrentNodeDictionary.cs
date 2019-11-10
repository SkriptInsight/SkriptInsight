using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using SkriptInsight.Core.Files.Nodes;

namespace SkriptInsight.Core.Extensions
{
    public class ConcurrentNodeDictionary : ConcurrentDictionary<int, AbstractFileNode>
    {
        public IEnumerable<AbstractFileNode> GetRange(int startInclusive, int endExclusive)
        {
            for (var i = startInclusive; i < endExclusive; i++)
            {
                var val = this[i];
                if (val != null) yield return val;
            }
        }
        
        public new AbstractFileNode this[int key]
        {
            get => this.ElementAtOrDefault(key).Value;
            set => base[key] = value;
        }
    }
}