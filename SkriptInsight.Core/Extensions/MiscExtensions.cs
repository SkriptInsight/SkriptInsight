using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Humanizer;
using JetBrains.Annotations;
using Newtonsoft.Json;
using SkriptInsight.Core.Files.Nodes;
using SkriptInsight.Core.Managers;
using SkriptInsight.Core.Types.Attributes;

namespace SkriptInsight.Core.Extensions
{
    public static class MiscExtensions
    {
        public static TV GetValueOrCompute<TK, TV>(this IDictionary<TK, TV> dict, TK key,
            [CanBeNull] Func<TK, TV> computeFunc = null)
        {
            var tryGetValue = dict.TryGetValue(key, out var value);
            if (tryGetValue || computeFunc == null) return tryGetValue ? value : default;

            var result = computeFunc(key);
            if (result != null)
                dict[key] = result;
            return result;
        }

        public static TV GetValue<TK, TV>(this ConcurrentDictionary<TK, TV> dict, TK key)
        {
            return dict.TryGetValue(key, out var value) ? value : default;
        }

        public static string SafeSubstring(this string value, int startIndex, int length)
        {
            return new string((value ?? string.Empty).Skip(startIndex).Take(length).ToArray());
        }

        public static string SafeSubstring(this string value, int startIndex)
        {
            return new string((value ?? string.Empty).Skip(startIndex).ToArray());
        }

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

        public static T FastFirstOrDefault<T>(this List<T> source, in bool shouldConsumeSource, ref int index)
        {
            if (shouldConsumeSource) index++;
            return source.Count > 0 ? source[0] : default;
        }


        public static List<T> FastSkip<T>(this List<T> source, int countToSkip)
        {
            var index = 0;
            return FastSkip(source, countToSkip, false, ref index);
        }

        public static List<T> FastSkip<T>(this List<T> source, int countToSkip, in bool shouldConsumeSource,
            ref int index)
        {
            var skipTotal = countToSkip /* + skipByConsume*/;
            return source.Count > countToSkip
                ? source.GetRange(skipTotal, source.Count - skipTotal)
                : new List<T>();
        }

        public static T FastElementAtOrDefault<T>(this T[] source, int index) =>
            source.Length >= index + 1 ? source[index] : default;

        public static T FastElementAtOrDefault<T>(this List<T> source, int index) =>
            source.Count >= index + 1 ? source[index] : default;


        public static List<ContextualElement<T>>
            WithContext<T>(this List<T> source, bool skipFirst = true, bool shouldConsumeSource = false)
        {
            var i = 0;
            var previous = default(T)!;
            var current = source.FastFirstOrDefault(shouldConsumeSource, ref i);
            var resultList = new List<ContextualElement<T>>();

            var toUse = source.FastSkip(skipFirst ? 1 : 0, shouldConsumeSource, ref i);

            toUse.Add(default);

            for (; i < toUse.Count; i++)
            {
                var next = toUse.FastElementAtOrDefault(i);
                if (current != null)
                    resultList.Add(new ContextualElement<T>(current, previous, next));
                previous = current;
                current = next;
            }

            return resultList;
        }

        public static T GetAttributeOfType<T>(this object enumVal) where T : Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString() ?? throw new InvalidOperationException(
                $"Parameter {nameof(enumVal)} cannot be null"));
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

            WorkspaceManager.CurrentHost.LogInfo($"Start nodes count [{lst.Count}]");
            for (var i = startRange; i <= endRange; i++)
            {
                var value = lst.ElementAtOrDefault(i).Value;
                if (value != null) update?.Invoke(value);
                lst[i - amount] = value;
            }
        }

        public static bool IsPlural(this string str)
        {
            return str.Pluralize(false) == str;
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

        public static string[] GetPatternAliases(this object value)
        {
            return value.GetAttributeOfType<PatternAliasAttribute>()?.Aliases ?? new string[0];
        }

        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
        }

        public static bool IsEmpty(this StringBuilder value)
        {
            return value.Length == 0;
        }

        public static string GetDescription<T>([NotNull] this T source)
        {
            var fi = source.GetType().GetField(source.ToString() ?? throw new InvalidOperationException());

            Debug.Assert(fi != null, nameof(fi) + " != null");
            var attributes = (DescriptionAttribute[]) fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : $"Message missing for code <{source}>";
        }

        [CanBeNull]
        public static string GetClassDescription<T>([NotNull] this T source)
        {
            var attribute = source.GetType().GetCustomAttribute<DescriptionAttribute>();

            return attribute?.Description;
        }
    }
}