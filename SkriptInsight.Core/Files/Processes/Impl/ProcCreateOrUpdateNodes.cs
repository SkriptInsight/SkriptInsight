using System.Linq;
using System.Reflection;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Files.Nodes;
using SkriptInsight.Core.Files.Nodes.Impl;
using SkriptInsight.Core.Managers;
using SkriptInsight.Core.Parser;
using SkriptInsight.Core.Utils;

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
            } else
            {
                var ctx = ParseContext.FromCode(rawContent);
                //Try to match to one of our known signatures
                foreach (var (signatureNodeType, signatureDelegate) in NodeSignaturesManager.Instance.SignatureTypes)
                {
                    ctx.Matches.Clear();
                    ctx.CurrentPosition = 0;
                    
                    if (resultNode.IsSectionNode != (signatureNodeType.GetCustomAttribute<SectionNodeAttribute>() != null)) continue;
                    
                    var tryParseResult = signatureDelegate.DynamicInvoke(ctx);

                    // We matched one signature
                    if (tryParseResult != null)
                    {
                        var instance = signatureNodeType.NewInstance(tryParseResult);

                        if (instance is AbstractFileNode fileNode)
                        {
                            NodeContentHelper.ApplyBasicNodeInfoToOtherNode(resultNode, ref fileNode);
                            resultNode = fileNode;
                        }

                        break;
                    }
                }
            }
            
            file.Nodes[lineNumber] = resultNode;
        }
    }
}