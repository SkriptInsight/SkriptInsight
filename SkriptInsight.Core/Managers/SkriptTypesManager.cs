using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using MoreLinq;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Files;
using SkriptInsight.Core.SyntaxInfo;
using SkriptInsight.JavaReader;

namespace SkriptInsight.Core.Managers
{
    public class SkriptTypesManager
    {
        [CanBeNull]
        public SkriptType GetType(string name)
        {
            return NamedKnownTypesFromAddons.GetValue(name);
        }

        [CanBeNull]
        public IReadOnlyList<SkriptExpression> GetExpressionsThatCanFitType(SkriptType type)
        {
            Debug.WriteLine($"Got query for GetExpressionsThatCanFitType({type.ClassName})");
            return ExpressionsForType.GetValue(type.ClassName);
        }

        public IReadOnlyList<SkriptExpression> KnownExpressionsFromAddons { get; set; }

        public IReadOnlyList<SkriptType> KnownTypesFromAddons { get; set; }

        private ConcurrentDictionary<string, SkriptType> NamedKnownTypesFromAddons { get; set; }

        private ConcurrentDictionary<string, IReadOnlyList<SkriptExpression>> ExpressionsForType { get; set; }

        private void LoadTypesFromAddons()
        {
            KnownTypesFromAddons =
                Workspace.AddonDocumentations.SelectMany(a => a.Types).ToList();

            LoadNamedTypes();
        }

        private void LoadNamedTypes()
        {
            NamedKnownTypesFromAddons = new ConcurrentDictionary<string, SkriptType>();
            foreach (var type in KnownTypesFromAddons)
            {
                NamedKnownTypesFromAddons.TryAdd(type.FinalTypeName, type);
            }
        }

        private void LoadExpressionsFromAddons()
        {
            var info = Workspace.AddonDocumentations
                .SelectMany(a => a.Expressions)
                .ToList();

            void RegisterEventValuesFor(SkriptEvent skriptEvent1, EventValueInfo[] values)
            {
                if (values != null)
                    info.AddRange(values.Select(x => x.ToEventExpression(this, skriptEvent1)));
            }

            foreach (var skriptEvent in Workspace.AddonDocumentations.SelectMany(c => c.Events))
            {
                RegisterEventValuesFor(skriptEvent, skriptEvent.PastEventValues);
                RegisterEventValuesFor(skriptEvent, skriptEvent.CurrentEventValues);
                RegisterEventValuesFor(skriptEvent, skriptEvent.FutureEventValues);
            }

            KnownExpressionsFromAddons = info.DistinctBy(c => c is SkriptEventValueExpression eventValueExpression ? (object)(eventValueExpression.Parent, eventValueExpression.RawName) : c).ToList();
        }

        private void MapExpressionsForSkriptTypes()
        {
            ExpressionsForType = new ConcurrentDictionary<string, IReadOnlyList<SkriptExpression>>();
            LoadMatchingExpressionsForTypes();
        }

        private void LoadMatchingExpressionsForTypes()
        {
            foreach (var (type, expressions) in WorkspaceManager.KnownTypesManager.WholeTypeCache
                .Where(c =>
                {
                    var (_, expression) = c;
                    return !expression.IsPlural;
                })
                .Select(cc => (Type: cc.Value.ClassName, Expressions: KnownExpressionsFromAddons
                    .Where(c => c.ReturnType == cc.Value.ClassName ||
                                CheckClassExtendsAnother(c.ReturnType, cc.Value.ClassName)).ToList())))
                ExpressionsForType.TryAdd(type, expressions);

            ExpressionsForType.TryAdd(KnownTypesManager.JavaLangObjectClass,
                KnownExpressionsFromAddons.ToList());
        }

        private bool CheckClassExtendsAnother(string returnType, string className)
        {
            var returnTypeClass = LoadedClassRepository.Instance[returnType];
            var classNameClass = LoadedClassRepository.Instance[className];

            if (returnTypeClass == null || classNameClass == null) return false;

            return returnTypeClass.InstanceOf(classNameClass);
        }

        public void InitTypesFromAddons(SkriptWorkspace workspace = null, WorkspaceManager workspaceManager = null)
        {
            Workspace = workspace ?? WorkspaceManager.CurrentWorkspace;
            WorkspaceManager = workspaceManager ?? WorkspaceManager.Instance;
            LoadTypesFromAddons();
        }

        public void LoadExpressionsFromTypes()
        {
            LoadExpressionsFromAddons();
            MapExpressionsForSkriptTypes();
        }

        public SkriptWorkspace Workspace { get; set; }

        public WorkspaceManager WorkspaceManager { get; set; }
    }
}