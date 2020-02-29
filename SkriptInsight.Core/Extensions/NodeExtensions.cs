using System;
using System.Dynamic;
using System.Linq;
using SkriptInsight.Core.Files.Nodes;
using SkriptInsight.Core.SyntaxInfo;

namespace SkriptInsight.Core.Extensions
{
    public static class NodeExtensions
    {
        private static readonly dynamic DynNameReturner = new DynamicNameReturner();

        public static AbstractFileNode FindRootParent(this AbstractFileNode node)
        {
            while (true)
            {
                if (node?.Parent == null) return node;
                node = node.Parent;
            }
        }

        public static AbstractFileNode FindBottomRootChildNode(this AbstractFileNode node)
        {
            try
            {
                var parent = node.FindRootParent();
                return parent.Children?.Count > 0 ? parent.Children[^1] : parent;
            }
            catch
            {
                return node;
            }
        }

        public static bool IsMatchOfType(this AbstractFileNode node, Func<dynamic, string> name)
        {
            return IsMatchOfType(node, name(DynNameReturner));
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static bool IsMatchOfType(this AbstractFileNode node, string name)
        {
            if (node?.MatchedSyntax?.Element == null) return false;
            switch (node.MatchedSyntax?.Element)
            {
                case SkriptCondition skriptCondition:
                    return GetClassName(skriptCondition.ClassName) == name;
                case SkriptEffect skriptEffect:
                    return GetClassName(skriptEffect.ClassName) == name;
                case SkriptEvent skriptEvent:
                    if (skriptEvent.ClassNames.Any(cl => GetClassName(cl) == name))
                        return true;
                    break;
                case SyntaxSkriptExpression skriptExpression:
                    return GetClassName(skriptExpression.ClassName) == name;
            }
            return false;
        }

        private static string GetClassName(string name)
        {
            var i = name.LastIndexOf('.');
            return string.Intern(i > -1 ? name.Substring(i + 1) : name);
        }

        private class DynamicNameReturner : DynamicObject
        {
            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                result = binder.Name;
                return true;
            }
        }
    }
}