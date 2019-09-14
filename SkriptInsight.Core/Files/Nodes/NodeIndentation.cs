namespace SkriptInsight.Core.Files.Nodes
{
    public class NodeIndentation
    {
        public NodeIndentation()
        {
        }

        public NodeIndentation(IndentType type, int count)
        {
            Type = type;
            Count = count;
        }

        public IndentType Type { get; set; } = IndentType.None;

        public int Count { get; set; }

        protected bool Equals(NodeIndentation other)
        {
            return Type == other.Type && Count == other.Count;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((NodeIndentation) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) Type * 397) ^ Count;
            }
        }

        public static NodeIndentation FromCharacter(char c, int count)
        {
            return new NodeIndentation
            {
                Type = GetIndentTypeFromChar(c),
                Count = count
            };
        }

        public static IndentType GetIndentTypeFromChar(char c)
        {
            return c switch
            {
                ' ' => IndentType.Space,
                '\t' => IndentType.Tab,
                _ => IndentType.Unknown
            };
        }
    }
}