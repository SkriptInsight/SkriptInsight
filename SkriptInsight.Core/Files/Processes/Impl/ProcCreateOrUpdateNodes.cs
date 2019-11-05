using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Files.Nodes;
using SkriptInsight.Core.Files.Nodes.Impl;

namespace SkriptInsight.Core.Files.Processes.Impl
{
    public class ProcCreateOrUpdateNodes : FileProcess
    {
        public override void DoWork(SkriptFile file, int lineNumber, string rawContent, FileParseContext context)
        {
            AbstractFileNode resultNode = new BaseFileNode();
            NodeContentHelper.ApplyBasicNodeInfoToNode(rawContent, lineNumber, file, ref resultNode);

            
            
            if (resultNode.NodeContent.IsEmpty() && !resultNode.RawComment.IsEmpty())
            {
                AbstractFileNode commentNode = new CommentNode();
                NodeContentHelper.ApplyBasicNodeInfoToOtherNode(resultNode, ref commentNode);
                resultNode = commentNode;
            } else if (resultNode.NodeContent.IsEmpty() && resultNode.RawComment.IsEmpty())
            {
                AbstractFileNode emptyLineNode = new EmptyLineNode();
                NodeContentHelper.ApplyBasicNodeInfoToOtherNode(resultNode, ref emptyLineNode);
                resultNode = emptyLineNode;
            }
            
            file.Nodes[lineNumber] = resultNode;
        }
    }
}