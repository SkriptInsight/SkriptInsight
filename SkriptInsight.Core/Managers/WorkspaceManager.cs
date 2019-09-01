using SkriptInsight.Core.Files;

namespace SkriptInsight.Core.Managers
{
    public class WorkspaceManager
    {
        private WorkspaceManager _instance;

        public WorkspaceManager Instance => _instance ??= new WorkspaceManager();

        public SkriptWorkspace Current { get; } = new SkriptWorkspace();

        public void HandleOpenedFile(SkriptFile file)
        {
            if (!Current.Files.Contains(file))
                Current.Files.Add(file);
        }

        public void HandleClosedFile(SkriptFile file)
        {
            if (Current.Files.Contains(file))
                Current.Files.Remove(file);
        }
    }
}