using System.Linq;
using SkriptInsight.Core.Files.Nodes;

namespace SkriptInsight.Core.Files.Processes.Impl
{
    public class ProcFileNodePreProcess : FileProcess
    {
        public ProcFileNodePreProcess(AbstractFileNode fileNode)
        {
            FileNode = fileNode;
        }

        public AbstractFileNode FileNode { get; set; }

        public override void DoWork(SkriptFile file, int lineNumber, string rawContent)
        {
            FileNode.Indentations = rawContent.TakeWhile(char.IsWhiteSpace).GroupBy(c => c)
                .Select(c => NodeIndentation.FromCharacter(c.Key, c.Count())).ToArray();
        }
    }
}