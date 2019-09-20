using System.Collections.Generic;
using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Parser.Patterns;
using SkriptInsight.Core.SyntaxInfo;

namespace SkriptInsight.Core.Files.Nodes
{
    public abstract class AbstractFileNode
    {
        /// <summary>
        /// The line number of this node. Zero based integer value.
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Source file
        /// </summary>
        [JsonIgnore]
        public SkriptFile File { get; set; }
        
        /// <summary>
        /// Parent node of this node
        /// </summary>
        [JsonIgnore]
        public AbstractFileNode Parent { get; set; }
        
        /// <summary>
        /// Children nodes of this node
        /// </summary>
        [JsonIgnore]
        public List<AbstractFileNode> Children { get; set; }
        
        /// <summary>
        /// Indentations of this node
        /// </summary>
        public NodeIndentation[] Indentations { get; set; } = new NodeIndentation[0];

        /// <summary>
        /// Range of this node
        /// </summary>
        public Range Range { get; set; }

        /// <summary>
        /// Range of the indentation of this node
        /// </summary>
        public Range IndentationRange { get; set; }
        
        /// <summary>
        /// The Raw Text content of this node.
        /// </summary>
        public string RawText { get; set; }
        
        /// <summary>
        /// Range of the comment of this node
        /// </summary>
        public Range CommentRange { get; set; }
        
        /// <summary>
        /// Range of the content of this node (doesn't include comments or indentation)
        /// </summary>
        public Range ContentRange { get; set; }
        
        /// <summary>
        /// Content of the comment of this node
        /// </summary>
        public string RawComment { get; set; }

        /// <summary>
        /// Text content of this node (doesn't include comments or indentation)
        /// </summary>
        public string NodeContent { get; set; }

        /// <summary>
        /// Check whether this node is a section node or not 
        /// </summary>
        public bool IsSectionNode { get; set; }

        public SyntaxMatch MatchedSyntax { get; set; }
    }
}