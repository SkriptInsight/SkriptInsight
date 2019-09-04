namespace SkriptInsight.Core.Files.Nodes
{
    public class NodeIndentation
    {
        public IndentType Type { get; set; } = IndentType.None;

        public int Count { get; set; }

        public static NodeIndentation FromCharacter(char c, int count)
        {
            return new NodeIndentation
            {
                Type = c switch
                {
                    ' ' => IndentType.Space,
                    '\t' => IndentType.Tab,
                    _ => IndentType.Unknown
                },
                Count = count
            };
        }
    }
}