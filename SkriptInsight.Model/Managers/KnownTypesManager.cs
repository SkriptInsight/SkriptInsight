using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using SkriptInsight.Model.Parser.Types;

namespace SkriptInsight.Model.Managers
{
    public class KnownTypesManager
    {
        public class KnownType
        {
            public KnownType(Type type)
            {
                Type = type;
                SkriptRepresentations = type.GetCustomAttributes<TypeDescriptionAttribute>().Select(c => c.TypeName).ToArray();
            }

            public string[] SkriptRepresentations { get; }

            public Type Type { get; }

            public ISkriptTypeBase CreateNewInstance() => (ISkriptTypeBase) Activator.CreateInstance(Type);
        }

        private static KnownTypesManager _instance;

        public static KnownTypesManager Instance => _instance ??= new KnownTypesManager();

        private KnownTypesManager()
        {
            LoadKnownTypes();
        }

        public List<KnownType> KnownTypes { get; set; }

        public void LoadKnownTypes()
        {
            if (KnownTypes == null)
                KnownTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(c => c.GetTypes())
                    .Where(c => c.IsSubclassOfRawGeneric(typeof(SkriptType<>)))
                    .Select(p => new KnownType(p)).ToList();
        }

        [CanBeNull]
        public KnownType GetTypeByName(string name)
        {
            return KnownTypes.FirstOrDefault(t => t.SkriptRepresentations.Contains(name));
        }
    }
}