using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Files.Nodes;
using SkriptInsight.Core.Files.Nodes.Impl;
using SkriptInsight.Core.Managers;
using SkriptInsight.Core.Parser;
using SkriptInsight.Core.Parser.Patterns;
using SkriptInsight.Core.Parser.Patterns.Impl;
using SkriptInsight.Core.SyntaxInfo;
using SkriptInsight.Core.Utils;

namespace SkriptInsight.Core.Files.Processes.Impl
{
    [Description("")]
    public class ProcCreateOrUpdateNodes : FileProcess
    {
        public override string ReportProgressTitle => this.GetClassDescription();
        public override string ReportProgressMessage => "Structurally parsing code";

        private static readonly OptionalPatternElement SectionPattern = new OptionalPatternElement
        {
            Element = new LiteralPatternElement(":")
        };
        public override void DoWork(SkriptFile file, int lineNumber, string rawContent, FileParseContext context)
        {
            AbstractFileNode resultNode = new BaseFileNode();
            NodeContentHelper.ApplyBasicNodeInfoToNode(rawContent, lineNumber, file, ref resultNode);

            if (resultNode.NodeContent.IsEmpty() && !resultNode.RawComment.IsEmpty())
            {
                AbstractFileNode commentNode = new CommentLineNode();
                NodeContentHelper.ApplyBasicNodeInfoToOtherNode(resultNode, ref commentNode);
                resultNode = commentNode;
            }
            else if (resultNode.NodeContent.IsEmpty() && resultNode.RawComment.IsEmpty())
            {
                AbstractFileNode emptyLineNode = new EmptyLineNode();
                emptyLineNode.MatchedSyntax =
                    new SyntaxMatch(SignatureElements.EmptyLine, ParseResult.Success(context));
                NodeContentHelper.ApplyBasicNodeInfoToOtherNode(resultNode, ref emptyLineNode);
                resultNode = emptyLineNode;
            }
            else
            {
                if (file.IsNodeVisible(resultNode))
                {
                    var ctx = ParseContext.FromCode(rawContent);
                    
                    var signatureMatches = new List<(bool isSectionMismatch, AbstractFileNode node)>();
                    //Try to match to one of our known signatures
                    foreach (var (signatureNodeType, signatureDelegate) in NodeSignaturesManager.Instance.SignatureTypes)
                    {
                        ctx.Matches.Clear();
                        ctx.CurrentPosition = context.IndentationChars;
                    
                        var isSectionTypeMismatch = resultNode.IsSectionNode !=
                                                    (signatureNodeType.GetCustomAttribute<SectionNodeAttribute>() != null);


                        var tryParseResult = signatureDelegate.DynamicInvoke(ctx);

                        if (tryParseResult != null)
                        {
                            //Try to match the section colon so that we reach the end of the line
                            SectionPattern.Parse(ctx);
                        }
                    
                        // We matched one signature
                        if (tryParseResult != null && ctx.HasFinishedLine)
                        {
                            var instance = signatureNodeType.NewInstance(tryParseResult);

                            if (instance is AbstractFileNode fileNode)
                            {
                                NodeContentHelper.ApplyBasicNodeInfoToOtherNode(resultNode, ref fileNode);
                                signatureMatches.Add((isSectionTypeMismatch, fileNode));
                            }

                            if (!isSectionTypeMismatch)
                                break;
                        }
                    }

                    var resultingNode = signatureMatches.FirstOrDefault(x => !x.isSectionMismatch).node;
                    if (resultingNode != null)
                    {
                        resultNode = resultingNode;
                    }
                }
            }

            file.Nodes[lineNumber] = resultNode;
        }
    }
}