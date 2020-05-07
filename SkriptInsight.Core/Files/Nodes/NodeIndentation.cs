using System;
using System.Collections.Generic;

namespace SkriptInsight.Core.Files.Nodes
{
    public class NodeIndentation : IEqualityComparer<NodeIndentation>
    {
        public bool Equals(NodeIndentation x, NodeIndentation y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Type == y.Type && x.Count == y.Count;
        }

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

        public int GetHashCode(NodeIndentation obj)
        {
            return HashCode.Combine((int) obj.Type, obj.Count);
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