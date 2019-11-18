using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using MoreLinq;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Parser.Patterns.Impl;
using SkriptInsight.Core.Parser.Types;
using SkriptInsight.Core.SyntaxInfo;

namespace SkriptInsight.Core.Managers
{
    public class KnownTypesManager
    {
        public WorkspaceManager WorkspaceManager { get; }

        internal KnownTypesManager(WorkspaceManager workspaceManager)
        {
            WorkspaceManager = workspaceManager;
            LoadKnownTypes();
            LoadTypesCache();
        }

        [Obsolete("Use the new property on WorkspaceManager", true)]
        public static KnownTypesManager Instance => WorkspaceManager.Instance.KnownTypesManager;

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

        public Dictionary<string, SkriptType> WholeTypeCache { get; set; }

        public void LoadTypesCache()
        {
            var cleanSkriptTypes = WorkspaceManager.Current.AddonDocumentations
                .SelectMany(c => c.Types).ToList();

            var extraSkriptTypes = WorkspaceManager.Current.AddonDocumentations
                .SelectMany(c => c.Effects).SelectMany(e => e.PatternNodes)
                .SelectMany(c => c.Children.OfType<TypePatternElement>()).Select(c => c.Type).ToList();

            WholeTypeCache = cleanSkriptTypes
                .Select(neededType => InternalGetTypeForName(neededType.FinalTypeName, cleanSkriptTypes))
                .Where(c => c != null)
                .Concat(extraSkriptTypes.SelectMany(c => c.Split('/'))
                    .Select(type => InternalGetTypeForName(type, cleanSkriptTypes)).Where(c => c != null))
                .DistinctBy(c => c.FinalTypeName)
                .ToDictionary(c => c.FinalTypeName, c => c);
        }

        private SkriptType InternalGetTypeForName(string name, IEnumerable<SkriptType> skriptTypes = null)
        {
            if (skriptTypes == null)
                skriptTypes = WorkspaceManager.Current.KnownTypesFromAddons;

            return skriptTypes
                .Where(type => type.FinalTypeName.Equals(name))
                .DefaultIfEmpty(
                    WorkspaceManager.Current.KnownTypesFromAddons
                        .Where(c => c.PatternsRegexps != null)
                        .FirstOrDefault(c =>
                            c.PatternsRegexps.Any(r => r.IsMatch(name)))
                ).FirstOrDefault();
        }

        [CanBeNull]
        public KnownType GetTypeByName(string name)
        {
            if (TypesLookupCache.ContainsKey(name))
                return TypesLookupCache[name];

            var patterns = WholeTypeCache.GetValueOrCompute(name, typeName => InternalGetTypeForName(typeName))
                ?.PatternsRegexps;

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