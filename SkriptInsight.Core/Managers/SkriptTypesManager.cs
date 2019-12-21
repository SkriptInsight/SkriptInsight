using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Files;
using SkriptInsight.Core.SyntaxInfo;

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
            KnownExpressionsFromAddons = Workspace.AddonDocumentations
                .SelectMany(a => a.Expressions).ToList();
        }

        private void MapExpressionsForSkriptTypes()
        {
            ExpressionsForType = new ConcurrentDictionary<string, IReadOnlyList<SkriptExpression>>();
            LoadMatchingExpressionsForTypes();
        }

        private void LoadMatchingExpressionsForTypes()
        {
            foreach (var (type, expressions) in WorkspaceManager.KnownTypesManager.WholeTypeCache
                .Where(c => !c.Value.IsPlural)
                .Select(cc => (Type: cc.Value.ClassName, Expressions: KnownExpressionsFromAddons
                    .Where(c => c.ReturnType == cc.Value.ClassName).ToList())))
                ExpressionsForType.TryAdd(type, expressions);

            ExpressionsForType.TryAdd(KnownTypesManager.JavaLangObjectClass,
                KnownExpressionsFromAddons.ToList());
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