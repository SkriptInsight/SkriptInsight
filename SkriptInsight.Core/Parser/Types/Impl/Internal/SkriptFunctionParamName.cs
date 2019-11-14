using System.Text;
using SkriptInsight.Core.Extensions;

namespace SkriptInsight.Core.Parser.Types.Impl.Internal
{
    [TypeDescription("si_func_param_name")]
    [InternalType]
    public class SkriptFunctionParamName : SkriptGenericType<string>
    {
        protected override string TryParse(ParseContext ctx)
        {
            var sb = new StringBuilder();
            var currArg = ctx.PeekNext(ctx.FindNextCharNotInsideNormalBracket(',', true, true) - ctx.CurrentPosition);

            for (var i = 0; i < currArg.Length; i++)
            {
                var c = currArg[i];
                if (c == ':' && currArg.IndexOf(':', i + 1) == -1) break;
                sb.Append(c);
            }

            ctx.ReadNext(sb.Length);

            return !sb.IsEmpty() ? sb.ToString() : null;
        }

        public override string AsString(string obj)
        {
            return obj;
        }
    }
}