using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SkriptInsight.Core.Files;

namespace SkriptInsight.Host.Lsp
{
    public class NoFileContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            
            if (property.PropertyType == typeof(SkriptFile) || property.PropertyType == typeof(FileParseContext))
            {
                property.ShouldSerialize = _ => false;
                property.Ignored = true;
            }
            return property;
        }
    }
}