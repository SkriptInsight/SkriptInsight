using System;
using Newtonsoft.Json;

namespace SkriptInsight.Model
{
    public static class Extensions
    {
        public static T JsonClone<T>(this T original)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(original, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.All
            }));
        }

        public static bool EqualsIgnoreCase(this string first, string second)
        {
            return string.Equals(first, second, StringComparison.OrdinalIgnoreCase);
        }
    }
}