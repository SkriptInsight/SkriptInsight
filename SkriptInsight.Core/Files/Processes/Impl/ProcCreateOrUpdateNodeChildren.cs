using System.Collections.Generic;
using System.Linq;
using SkriptInsight.Core.Files.Nodes;
using SkriptInsight.Core.Files.Nodes.Impl;

namespace SkriptInsight.Core.Files.Processes.Impl
{
    public class ProcCreateOrUpdateNodeChildren : FileProcess
    {
        public int CurrentLevel { get; }

        public ProcCreateOrUpdateNodeChildren(in int currentLevel)
        {
            CurrentLevel = currentLevel;
        }

        public override void DoWork(SkriptFile file, int lineNumber, string rawContent, FileParseContext context)
        {
            var nodes = file.Nodes;
            var node = nodes[lineNumber];

            if (node != null && node.IsSectionNode && IsCurrentNodeOnSameIndentLevel(node.Indentations))
            {
                for (var i = lineNumber + 1;
                    i < nodes.Count && IsChildrenAccordingToIndent(nodes[i], CurrentLevel);
                    i++)
                {
                    var nextNode = nodes[i];
                    if (nextNode == null) continue;

                    nextNode.Parent?.Children?.Remove(node);
                    nextNode.Parent = node;
                    node.Children.Add(nextNode);
                }
            }
        }

        internal static bool IsChildrenAccordingToIndent(AbstractFileNode node, int level)
        {
            return (GetIndentCount(node) > level || node is EmptyLineNode || node is CommentLineNode);
        }

        internal static int GetIndentCount(AbstractFileNode node)
        {
            return node?.Indentations?.Select(c => (c.Type == IndentType.Tab ? 4 : 1) * c.Count).Sum() ?? 0;
        }

        private bool IsCurrentNodeOnSameIndentLevel(IReadOnlyCollection<NodeIndentation> indentations)
        {
            if (indentations.Count > 1) return false;
            if (indentations.Count == 0 && CurrentLevel == 0) return true;

            return indentations.FirstOrDefault()?.Count == CurrentLevel;
        }
    }
}