using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using SkriptInsight.Core.Files.Nodes;
using SkriptInsight.Core.Managers;
using SkriptInsight.Core.Parser.Patterns;
using SkriptInsight.Core.Parser.Patterns.Impl;
using SkriptInsight.Core.Types.Attributes;

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

        public static T GetAttributeOfType<T>(this object enumVal) where T : Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return attributes.Length > 0 ? (T) attributes[0] : null;
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

        public static void ShiftRangeRight<T>(this ConcurrentDictionary<int, T> lst, int startRange, int count,
            int amount, Action<T> update = null)
        {
            var endRange = startRange + count;
            for (var i = endRange - amount; i >= startRange; i--)
            {
                if (i > (endRange + amount)) continue;
                var value = lst.ElementAtOrDefault(i).Value;
                
                if (value != null) update?.Invoke(value);
                lst[i + amount] = value;
            }
        }

        public static void ShiftRangeLeft<T>(this ConcurrentDictionary<int, T> lst, int startRange, int count,
            int amount, Action<T> update = null)
        {
            var endRange = startRange + count;

            WorkspaceManager.Instance.Current.Server.Window.LogInfo($"Start nodes count [{lst.Count}]");
            for (int i = startRange; i <= endRange; i++)
            {
                var value = lst.ElementAtOrDefault(i).Value;
                if (value != null) update?.Invoke(value);
                lst[i - amount] = value;
            }
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
        
        public static string[] GetAliases(this object value)
        {
            return value.GetAttributeOfType<PatternAliasAttribute>()?.Aliases ?? new string[0];
        }
        
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }
        
    }
}