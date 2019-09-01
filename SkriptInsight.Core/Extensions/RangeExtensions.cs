using System.Collections.Generic;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace SkriptInsight.Core.Extensions
{
    public static class RangeExtensions
    {
        public static int ResolveFor(this Position pos, List<string> str)
        {
            var charsUntilLine = 0;
            for (var index = 0; index < str.Count; index++)
            {
                var line = str[index];
                if (index < pos.Line)
                {
                    charsUntilLine += line.Length;
                }
                else
                {
                    charsUntilLine += (int) pos.Character;
                    break;
                }
            }

            return charsUntilLine;
        }
    }
}