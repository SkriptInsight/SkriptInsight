using SkriptInsight.Core.Parser.Patterns;

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
        public SkriptFile File { get; set; }

        /// <summary>
        /// Indentations of this node
        /// </summary>
        public NodeIndentation[] Indentations { get; set; } = new NodeIndentation[0];

        /// <summary>
        /// The Raw Text content of this node.
        /// </summary>
        public string RawText { get; set; }
    }
}