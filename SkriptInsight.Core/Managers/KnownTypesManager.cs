using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Parser.Types;

namespace SkriptInsight.Core.Managers
{
    public class KnownTypesManager
    {
        private KnownTypesManager()
        {
            LoadKnownTypes();
        }

        public static KnownTypesManager Instance { get; } = new KnownTypesManager();

        public List<KnownType> KnownTypes { get; set; }

        private ConcurrentDictionary<string, KnownType> TypesLookupCache { get; } =
            new ConcurrentDictionary<string, KnownType>();

        public void LoadKnownTypes()
        {
            if (KnownTypes == null)
                KnownTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(c => c.GetTypes())
                    .Where(c => c.IsSubclassOfRawGeneric(typeof(SkriptGenericType<>)))
                    .Select(p => new KnownType(p)).ToList();
        }

        [CanBeNull]
        public KnownType GetTypeByName(string name)
        {
            if (TypesLookupCache.ContainsKey(name))
                return TypesLookupCache[name];
            
            var patterns = WorkspaceManager.CurrentWorkspace.AddonDocumentations
                .SelectMany(c => c.Types).Where(t => t.FinalTypeName.Equals(name))
                .DefaultIfEmpty(
                    WorkspaceManager
                        .CurrentWorkspace.AddonDocumentations.SelectMany(c => c.Types)
                        .Where(c => c.PatternsRegexes != null)
                        .FirstOrDefault(c => c.PatternsRegexes.Any(r => r.IsMatch(name)))
                ).FirstOrDefault()?.PatternsRegexes;

            var result = KnownTypes.FirstOrDefault(c => c.SkriptRepresentations.Contains(name)) ??
                         KnownTypes.FirstOrDefault(t =>
                             patterns != null
                                 ? t.SkriptRepresentations.Any(r => patterns.Any(p => p.IsMatch(r)))
                                 : t.SkriptRepresentations.Contains(name));

            if (result != null)
                TypesLookupCache[name] = result;

            return result;
        }

        public class KnownType
        {
            public KnownType(Type type)
            {
                Type = type;
                SkriptRepresentations = type.GetCustomAttributes<TypeDescriptionAttribute>().Select(c => c.TypeName)
                    .ToArray();
            }

            public string[] SkriptRepresentations { get; }

            public Type Type { get; }

            public ISkriptType CreateNewInstance()
            {
                return (ISkriptType) Activator.CreateInstance(Type);
            }
        }
    }
}