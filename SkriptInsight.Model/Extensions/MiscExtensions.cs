using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SkriptInsight.Model.Extensions
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


        public static IEnumerable<ContextualElement<T>>
            WithContext<T>(this IEnumerable<T> source)
        {
            var previous = default(T);
            var current = source.FirstOrDefault();

            foreach (var next in source.Concat(new[] {default(T)}).Skip(1))
            {
                yield return new ContextualElement<T>(current, previous, next);
                previous = current;
                current = next;
            }
        }

        public static T JsonClone<T>(this T original)
        {
            return JsonConvert.DeserializeObject<T>(ToJson(original), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
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