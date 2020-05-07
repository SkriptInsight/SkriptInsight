using System.Collections.Generic;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using MoreLinq;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Parser;
using SkriptInsight.Core.SyntaxInfo;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace SkriptInsight.Core.Files.Nodes
{
    public abstract class AbstractFileNode
    {
        /// <summary>
        ///     The line number of this node. Zero based integer value.
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        ///     Source file
        /// </summary>
        [JsonIgnore]
        public SkriptFile File { get; set; }

        /// <summary>
        ///     Parent node of this node
        /// </summary>
        [JsonIgnore]
        public AbstractFileNode Parent { get; set; }

        /// <summary>
        ///     Children nodes of this node
        /// </summary>
        [JsonIgnore]
        public List<AbstractFileNode> Children { get; } = new List<AbstractFileNode>();

        /// <summary>
        ///     Indentations of this node
        /// </summary>
        public NodeIndentation[] Indentations { get; set; } = new NodeIndentation[0];

        /// <summary>
        ///     Range of this node
        /// </summary>
        public Range Range { get; set; }

        /// <summary>
        ///     Range of the indentation of this node
        /// </summary>
        public Range IndentationRange { get; set; }

        /// <summary>
        ///     The Raw Text content of this node.
        /// </summary>
        public string RawText { get; set; }

        /// <summary>
        ///     Range of the comment of this node
        /// </summary>
        public Range CommentRange { get; set; }

        /// <summary>
        ///     Range of the content of this node (doesn't include comments or indentation)
        /// </summary>
        public Range ContentRange { get; set; }

        /// <summary>
        ///     Content of the comment of this node
        /// </summary>
        public string RawComment { get; set; }

        /// <summary>
        ///     Text content of this node (doesn't include comments or indentation)
        /// </summary>
        public string NodeContent { get; set; }

        /// <summary>
        ///     Check whether this node is a section node or not
        /// </summary>
        public bool IsSectionNode { get; set; }

        public SyntaxMatch MatchedSyntax { get; set; }

        public override string ToString()
        {
            return RawText;
        }

        public void ShiftLineNumber(int amount)
        {
            LineNumber += amount;
            Range?.ShiftLineNumber(amount);
            CommentRange?.ShiftLineNumber(amount);
            ContentRange?.ShiftLineNumber(amount);
            IndentationRange?.ShiftLineNumber(amount);
            MatchedSyntax?.Result?.Matches?.ForEach(m =>
            {
                var expression = (m as ExpressionParseMatch)?.Expression;
                if (expression != null)
                    expression.GetAllExpressions().ForEach(expr => expr.Range.ShiftLineNumber(amount));
                else
                    m.Range.ShiftLineNumber(amount);
            });
            
            
        }

        [JsonIgnore]
        public AbstractFileNode RootParent => this.FindRootParent();

        [CanBeNull]
        public SyntaxMatch RootParentSyntax => RootParent?.MatchedSyntax;
    }
}