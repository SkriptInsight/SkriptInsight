using System.Collections.Generic;
using System.Linq;
using SkriptInsight.Core.Files.Nodes;
using SkriptInsight.Core.Files.Nodes.Impl;
using SkriptInsight.Core.Managers;

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
                    i < nodes.Count && (GetIndentCount(nodes[i]) > CurrentLevel || nodes[i] is EmptyLineNode ||
                                        nodes[i] is CommentLineNode);
                    i++)
                {
                    var nextNode = nodes[i];
                    if (nextNode == null) continue;
                    
                    //Remove this node and children from parent.
                    if (nextNode.Parent != null)
                    {
                        nextNode.Parent.Children.Remove(node);
                        nextNode.Parent.Children.RemoveAll(nextNode.Children.Contains);
                    }
                    nextNode.Parent = node;
                    node.Children.Add(nextNode);
                }
            }
        }

        private int GetIndentCount(AbstractFileNode node)
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