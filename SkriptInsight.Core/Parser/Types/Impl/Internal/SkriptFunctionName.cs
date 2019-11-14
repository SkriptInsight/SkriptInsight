using System;
using System.Text;
using SkriptInsight.Core.Extensions;

namespace SkriptInsight.Core.Parser.Types.Impl.Internal
{
    [InternalType]
    [TypeDescription("si_func_name")]
    public class SkriptFunctionName : SkriptGenericType<string>
    {
        protected override string TryParse(ParseContext ctx)
        {
            var sb = new StringBuilder();
            var conditionForMatch =
                new Func<char, bool>(c => sb.IsEmpty() ? char.IsLetter(c) : char.IsLetterOrDigit(c) || c == '_');
            var clone = ctx.Clone();
            
            foreach (var c in clone)
            {
                if (!conditionForMatch(c)) break;
                sb.Append(c);
            }

            if (!sb.IsEmpty()) ctx.ReadUntilPosition(clone.CurrentPosition - 1);
            

            return sb.ToString().IsEmpty() ? null : sb.ToString();
        }

        public override string RenderAsString(string obj)
        {
            return obj;
        }
    }
}