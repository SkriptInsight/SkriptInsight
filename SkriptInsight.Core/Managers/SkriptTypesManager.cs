using System.Collections.Concurrent;
using System.Collections.Generic;
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
        public const string ExprEntityClassName = "ch.njol.skript.expressions.ExprEntity";
        public const string ExprEntitiesClassName = "ch.njol.skript.expressions.ExprEntities";
        public const string ExprLoopValueClassName = "ch.njol.skript.expressions.ExprLoopValue";

        public static readonly string[] BannedClassNames =
        {
            ExprEntityClassName,
            ExprEntitiesClassName,
            ExprLoopValueClassName
        };

        public IReadOnlyList<SyntaxSkriptExpression> KnownExpressionsFromAddons { get; set; }

        public IReadOnlyList<SkriptType> KnownTypesFromAddons { get; set; }

        private ConcurrentDictionary<string, SkriptType> NamedKnownTypesFromAddons { get; set; }

        private ConcurrentDictionary<string, IReadOnlyList<SyntaxSkriptExpression>> ExpressionsForType { get; set; }

        //Just like above, but instead we have <Event id, Dictionary<Return type, Expression>>
        private ConcurrentDictionary<SkriptEvent, ConcurrentDictionary<string, IReadOnlyList<SyntaxSkriptExpression>>>
            EventExpressionsForType { get; set; }

        public SkriptWorkspace Workspace { get; set; }

        public WorkspaceManager WorkspaceManager { get; set; }

        [CanBeNull]
        public SkriptType GetType(string name)
        {
            return NamedKnownTypesFromAddons.GetValue(name);
        }

        [CanBeNull]
        public IReadOnlyList<SyntaxSkriptExpression> GetExpressionsThatCanFitType(SkriptType type)
        {
            return GetExpressionsThatCanFitType(type.ClassName);
        }

        [CanBeNull]
        public IReadOnlyList<SyntaxSkriptExpression> GetExpressionsThatCanFitType(string type)
        {
            return ExpressionsForType.GetValue(type);
        }


        [CanBeNull]
        public ConcurrentDictionary<string, IReadOnlyList<SyntaxSkriptExpression>> GetEventExpressionsForEvent(
            SkriptEvent @event)
        {
            return EventExpressionsForType.GetValue(@event);
        }

        private void LoadTypesFromAddons()
        {
            KnownTypesFromAddons =
                Workspace.AddonDocumentations.SelectMany(a => a.Types).ToList();

            LoadNamedTypes();
        }

        private void LoadNamedTypes()
        {
            NamedKnownTypesFromAddons = new ConcurrentDictionary<string, SkriptType>();
            foreach (var type in KnownTypesFromAddons) NamedKnownTypesFromAddons.TryAdd(type.FinalTypeName, type);
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

            KnownExpressionsFromAddons = info
                .Where(c => !BannedClassNames.Contains(c.ClassName))
                .DistinctBy(c =>
                    c is SyntaxSkriptEventValueExpression eventValueExpression
                        ? (object) (eventValueExpression.Parent, eventValueExpression.RawName)
                        : c)
                .ToList();
        }

        private void MapExpressionsForSkriptTypes()
        {
            ExpressionsForType = new ConcurrentDictionary<string, IReadOnlyList<SyntaxSkriptExpression>>();
            EventExpressionsForType =
                new ConcurrentDictionary<SkriptEvent,
                    ConcurrentDictionary<string, IReadOnlyList<SyntaxSkriptExpression>>>();
            LoadMatchingExpressionsForTypes();

            GenericExpressions = Workspace.AddonDocumentations.SelectMany(c => c.Expressions)
                .Where(c => c.ReturnType == KnownTypesManager.JavaLangObjectClass).ToList();
        }

        public IReadOnlyList<SyntaxSkriptExpression> GenericExpressions { get; set; }

        private void LoadMatchingExpressionsForTypes()
        {
            var nonPluralTypes = WorkspaceManager.KnownTypesManager.WholeTypeCache
                .Where(c =>
                {
                    var (_, expression) = c;
                    return !expression.IsPlural;
                }).ToList();
            foreach (var (type, expressions) in nonPluralTypes
                .Select(cc => (Type: cc.Value.ClassName, Expressions: KnownExpressionsFromAddons
                    .Where(c => !(c is SyntaxSkriptEventValueExpression))
                    .Where(c => !BannedClassNames.Contains(c.ClassName))
                    .Where(c =>
                        c.ReturnType == cc.Value.ClassName ||
                        CheckClassExtendsAnother(c.ReturnType, cc.Value.ClassName)).ToList())))
                ExpressionsForType.TryAdd(type, expressions);

            ExpressionsForType.TryAdd(KnownTypesManager.JavaLangObjectClass,
                KnownExpressionsFromAddons.ToList());

            LoadEventExpressionsForTypes(nonPluralTypes);
        }

        private void LoadEventExpressionsForTypes(IReadOnlyCollection<KeyValuePair<string, SkriptType>> nonPluralTypes)
        {
            foreach (var skriptEvent in Workspace.AddonDocumentations.SelectMany(c => c.Events))
            {
                var types = nonPluralTypes
                    .Select(cc => (Type: cc.Value.ClassName, Expressions: KnownExpressionsFromAddons
                        .OfType<SyntaxSkriptEventValueExpression>()
                        .Where(c => c.Parent == skriptEvent)
                        .Where(c => c.ReturnType == cc.Value.ClassName || CheckClassExtendsAnother(c.ReturnType,
                            cc.Value.ClassName)).ToList()))
                    .Where(c => c.Expressions.Count > 0)
                    .Select(c =>
                        new KeyValuePair<string, IReadOnlyList<SyntaxSkriptExpression>>(c.Type, c.Expressions))
                    .ToList();

                EventExpressionsForType.TryAdd(skriptEvent,
                    new ConcurrentDictionary<string, IReadOnlyList<SyntaxSkriptExpression>>(types));
            }
        }

        private bool CheckClassExtendsAnother(string returnType, string className)
        {
            var returnTypeClass = LoadedClassRepository.Instance[returnType];
            var classNameClass = LoadedClassRepository.Instance[className];

            if (returnTypeClass == null || classNameClass == null) return false;

            return returnTypeClass.InstanceOf(classNameClass) ||
                   returnType != KnownTypesManager.JavaLangObjectClass && classNameClass.InstanceOf(returnTypeClass);
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
    }
}