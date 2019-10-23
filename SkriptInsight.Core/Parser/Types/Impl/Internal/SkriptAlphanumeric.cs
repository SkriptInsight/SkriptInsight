using System.Text;
using SkriptInsight.Core.Extensions;

namespace SkriptInsight.Core.Parser.Types.Impl.Internal
{
    [InternalType]
    [TypeDescription("si_alphanumeric")]
    public class SkriptAlphanumeric : SkriptGenericType<string>
    {
        protected override string TryParse(ParseContext ctx)
        {
            var sb = new StringBuilder();
            foreach (var c in ctx)
            {
                if (!char.IsLetterOrDigit(c)) break;
                sb.Append(c);
            }
            return sb.ToString().IsEmpty() ? null : sb.ToString();
        }

        public override string AsString(string obj)
        {
            return obj;
        }
    }
}