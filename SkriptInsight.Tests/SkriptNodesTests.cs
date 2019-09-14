using System;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Files;
using SkriptInsight.Core.Files.Nodes;
using SkriptInsight.Core.Files.Nodes.Impl;
using Xunit;

namespace SkriptInsight.Tests
{
    public class SkriptNodesTests
    {
        [Fact]
        public void BasicNodeInformationIsCorrect()
        {
            AbstractFileNode resultNode = new UnknownFileNode();
            const string rawContent = @"    this is a##test #comment";
            const int lineNumber = 0;
            var expectedContentRange = RangeExtensions.From(lineNumber, 4, 20);
            var expectedCommentRange = RangeExtensions.From(lineNumber, 20, 28);
            var expectedIndentationRange = RangeExtensions.From(lineNumber, 0, 4);
            
            var file = new SkriptFile(new Uri("memory://tests"));
            NodeContentHelper.ApplyBasicNodeInfoToNode(rawContent, lineNumber, file, ref resultNode);
            
            Assert.Equal(expectedContentRange, resultNode.ContentRange);
            Assert.Equal(expectedCommentRange, resultNode.CommentRange);
            Assert.Equal(expectedIndentationRange, resultNode.IndentationRange);
            Assert.Equal(new [] {new NodeIndentation(IndentType.Space, 4)}, resultNode.Indentations);
            Assert.Single(resultNode.Indentations);
        }
    }
}