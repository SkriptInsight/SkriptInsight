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
                    return skriptCondition.ClassName == name;
                case SkriptEffect skriptEffect:
                    return skriptEffect.ClassName == name;
                case SkriptEvent skriptEvent:
                    if (skriptEvent.ClassNames.Any(cl => cl == name))
                        return true;
                    break;
                case SkriptExpression skriptExpression:
                    return skriptExpression.ClassName == name;
            }

            return false;
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