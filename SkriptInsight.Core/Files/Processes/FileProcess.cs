namespace SkriptInsight.Core.Files.Processes
{
    public abstract class FileProcess
    {
        public abstract void DoWork(SkriptFile file, int lineNumber, string rawContent);
    }
}