using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using SkriptInsight.Core.Inspections.Impl;

namespace SkriptInsight.Core.Inspections
{
    public class InspectionsManager
    {
        public InspectionsManager()
        {
            CodeInspections = new ConcurrentDictionary<Type, BaseInspection>(
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(c => c.GetTypes())
                    .Where(c => c.IsClass && !c.IsAbstract)
                    .Where(c => c.IsSubclassOf(typeof(BaseInspection)))
                    .Select(c => (Type: c, Description: c.GetCustomAttribute<InspectionDescriptionAttribute>()))
                    .Where(c => c.Description != null)
                    .ToDictionary(c => c.Type, c => Activator.CreateInstance(c.Type) as BaseInspection)
            );
        }

        public ConcurrentDictionary<Type, BaseInspection> CodeInspections { get; }
    }
}