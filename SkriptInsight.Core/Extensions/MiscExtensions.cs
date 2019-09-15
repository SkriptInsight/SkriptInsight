using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Force.DeepCloner;
using Newtonsoft.Json;
using SkriptInsight.Core.Files;
using SkriptInsight.Core.Files.Nodes;
using SkriptInsight.Core.Parser;

namespace SkriptInsight.Core.Extensions
{
    public static class MiscExtensions
    {
        public static List<string> SplitOnNewLines(this string str)
        {
            return str.Split(
                new[] {"\r\n", "\r", "\n"},
                StringSplitOptions.None
            ).ToList();
        }

        public static string[] SplitOnNewLinesArray(this string str)
        {
            return str.Split(
                new[] {"\r\n", "\r", "\n"},
                StringSplitOptions.None
            ).ToArray();
        }

        public static NodeIndentation[] GetNodeIndentations(this string text)
        {
            return text.TakeWhile(char.IsWhiteSpace).GroupBy(c => c)
                .Select(c => NodeIndentation.FromCharacter(c.Key, c.Count())).OrderBy(c => c.Type).ToArray();
        }
        
        public static void Resize<T>(this List<T> list, int sz)
        {
            if (list.Capacity < sz)
                list.Capacity = sz + 1;
        }

        public static IEnumerable<ContextualElement<T>>
            WithContext<T>(this IEnumerable<T> source, bool skipFirst = true)
        {
            var previous = default(T)!;
            var current = source.FirstOrDefault();

            var values = source.Concat(new[] {default(T)!});
            if (skipFirst)
                values = values.Skip(1);
            foreach (var next in values)
            {
                yield return new ContextualElement<T>(current, previous, next);
                previous = current;
                current = next;
            }
        }

        public static T JsonClone<T>(this T original)
        {
            var resultClone = original.DeepClone();
            if (resultClone is FileParseContext fileParseContext && original is FileParseContext fileOriginalContext)
            {
                fileParseContext.File = fileOriginalContext.File;
            }
            Debug.WriteLine($"Cloned object of type {typeof(T).Name}; {original.GetType().Name}");
            return resultClone;
        }

        public static string ToJson<T>(this T original)
        {
            return JsonConvert.SerializeObject(original, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                TypeNameHandling = TypeNameHandling.All
            });
        }

        public static bool EqualsIgnoreCase(this string first, string second)
        {
            return string.Equals(first, second, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsSubclassOfRawGeneric(this Type toCheck, Type generic)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }

                toCheck = toCheck.BaseType;
            }

            return false;
        }
    }
}