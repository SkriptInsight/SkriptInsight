using System.Linq;
using System.Text.RegularExpressions;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Files.Nodes.Impl;

namespace SkriptInsight.Core.Files.Nodes
{
    public class NodeContentHelper
    {
        private static readonly Regex LineRegex =
            new Regex(@"^((?:[^#]|##)*)(\s*#(?!#).*)$", RegexOptions.Compiled);

        public static void ApplyBasicNodeInfoToNode(string content, int line, SkriptFile file,
            ref AbstractFileNode node)
        {
            ExtractBasicNodeInformationFrom(content, line, out var indentations, out var indentRange,
                out var contentRange, out var nodeRange, out var commentRange, out var commentContent);

            node.RawText = content;
            node.LineNumber = line;
            node.Indentations = indentations;
            node.IndentationRange = indentRange;
            node.Range = nodeRange;
            node.CommentRange = commentRange;
            node.RawComment = commentContent;
            node.ContentRange = contentRange;
        }

        private static void ExtractBasicNodeInformationFrom(string content, int line,
            out NodeIndentation[] indentations, out Range indentRange, out Range contentRange,
            out Range nodeRange, out Range commentRange, out string commentContent)
        {
            var length = content.Length;
            var indentCharsCount = content.TakeWhile(char.IsWhiteSpace).Count();
            indentations = content.GetNodeIndentations();
            
            nodeRange = RangeExtensions.From(line, 0, content.Length);

            indentRange = RangeExtensions.From(line, 0, indentCharsCount);
            
            commentRange = RangeExtensions.From(line, length, length);
            
            commentContent = string.Empty;

            var matchResult = LineRegex.Match(content);
            if (matchResult.Success)
            {
                var contentGroup = matchResult.Groups[1];
                var commentGroup = matchResult.Groups[2];

                commentContent = commentGroup.Value;
                length = contentGroup.Length;
                commentRange.Start.Character = commentGroup.Index;
                commentRange.End.Character = commentGroup.Index + commentGroup.Length;
            }

            contentRange = RangeExtensions.From(line, indentCharsCount, length);
        }
    }
}