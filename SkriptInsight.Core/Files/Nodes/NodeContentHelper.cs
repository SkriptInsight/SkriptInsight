using System;
using System.Linq;
using System.Text.RegularExpressions;
using SkriptInsight.Core.Extensions;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace SkriptInsight.Core.Files.Nodes
{
    public static class NodeContentHelper
    {
        private static readonly Regex LineRegex =
            new Regex(@"^((?:[^#]|##)*)(\s*#(?!#).*)$", RegexOptions.Compiled);

        public static void ApplyBasicNodeInfoToNode(string content, int line, SkriptFile file,
            ref AbstractFileNode node)
        {
            ExtractBasicNodeInformationFrom(content, line, out var indentations, out var indentRange,
                out var contentRange, out var nodeRange, out var commentRange, out var commentContent,
                out var nodeContent);

            node.RawText = content;
            node.LineNumber = line;
            node.Indentations = indentations;
            node.IndentationRange = indentRange;
            node.Range = nodeRange;
            node.CommentRange = commentRange;
            node.RawComment = commentContent;
            node.ContentRange = contentRange;
            node.NodeContent = nodeContent;
            node.File = file;

            if (nodeContent.EndsWith(":"))
            {
                node.IsSectionNode = true;
            }
        }


        public static void ApplyBasicNodeInfoToOtherNode(AbstractFileNode original, ref AbstractFileNode target)
        {
            if (original.RawText != null) target.RawText = original.RawText;
            target.LineNumber = original.LineNumber;
            if (original.Indentations != null) target.Indentations = original.Indentations;
            if (original.IndentationRange != null) target.IndentationRange = original.IndentationRange;
            if (original.Range != null) target.Range = original.Range;
            if (original.CommentRange != null) target.CommentRange = original.CommentRange;
            if (original.RawComment != null) target.RawComment = original.RawComment;
            if (original.ContentRange != null) target.ContentRange = original.ContentRange;
            if (original.NodeContent != null) target.NodeContent = original.NodeContent;
            if (original.MatchedSyntax != null) target.MatchedSyntax = original.MatchedSyntax;
            if (original.File != null) target.File = original.File;
            target.IsSectionNode = original.IsSectionNode;
        }

        private static void ExtractBasicNodeInformationFrom(string content, int line,
            out NodeIndentation[] indentations, out Range indentRange, out Range contentRange,
            out Range nodeRange, out Range commentRange, out string commentContent, out string nodeContent)
        {
            var length = content.Length;
            var indentCharsCount = content.TakeWhile(char.IsWhiteSpace).Count();
            indentations = content.GetNodeIndentations();
            nodeContent = content;

            nodeRange = RangeExtensions.From(line, 0, content.Length);

            indentRange = RangeExtensions.From(line, 0, indentCharsCount);

            commentRange = RangeExtensions.From(line, length, length);

            commentContent = string.Empty;

            var matchResult = LineRegex.Match(content);
            if (matchResult.Success)
            {
                var contentGroup = matchResult.Groups[1];
                var commentGroup = matchResult.Groups[2];

                var endingSpaces = Math.Clamp(contentGroup.Length - contentGroup.Value.TrimEnd().Length, 0,
                    int.MaxValue);

                nodeContent = contentGroup.Value;
                commentContent = commentGroup.Value;
                length = contentGroup.Value.TrimEnd().Length;
                commentRange.Start.Character = commentGroup.Index - endingSpaces;
                commentRange.End.Character = (commentGroup.Index - endingSpaces) + (commentGroup.Length + endingSpaces);
            }

            nodeContent = nodeContent.Trim();
            contentRange = RangeExtensions.From(line, indentCharsCount, length);
        }
    }
}