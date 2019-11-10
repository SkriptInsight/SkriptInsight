using System.Collections.Concurrent;
using System.Linq;
using SkriptInsight.Core.Files.Nodes;

namespace SkriptInsight.Core.Extensions
{
    public class ConcurrentNodeDictionary : ConcurrentDictionary<int, AbstractFileNode>
    {
        public new AbstractFileNode this[int key]
        {
            get => this.ElementAtOrDefault(key).Value;
            set => base[key] = value;
        }
    }
}