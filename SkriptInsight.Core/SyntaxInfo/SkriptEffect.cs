using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using SkriptInsight.Core.Parser.Patterns;

namespace SkriptInsight.Core.SyntaxInfo
{
    [DebuggerDisplay("{" + nameof(ClassName) + "}")]
    public class SkriptEffect : AbstractSyntaxElement
    {
        public string ClassName { get; set; }
    }
}