using System.Reflection;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Files.Nodes;
using SkriptInsight.Core.Utils;

namespace SkriptInsight.Core.SyntaxNodes
{
    public static class SyntaxNodesExtensions
    {
        public static T GetSyntaxNode<T>(this AbstractFileNode node) where T: BaseSyntaxNode
        {
            var info = typeof(T).GetCustomAttribute<SyntaxInfoAttribute>();
            if (info != null && node.IsMatchOfType(info.ClassName))
            {
                return typeof(T).NewInstance(node, node.MatchedSyntax) as T;
            }
            return null;
        }
    }
}