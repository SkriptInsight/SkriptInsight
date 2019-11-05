using System;
using System.Linq;
using SkriptInsight.Core.Files;

namespace SkriptInsight.Core.Managers
{
    public class WorkspaceManager
    {
        public static ISkriptInsightHost CurrentHost { get; set; }
        
        public static WorkspaceManager Instance { get; } = new WorkspaceManager();
        
        public SkriptWorkspace Current { get; } = new SkriptWorkspace();
        
        public static SkriptWorkspace CurrentWorkspace => Instance.Current;

        public SkriptFile GetOrCreateByUri(Uri uri)
        {
            return Current.Files.FirstOrDefault(f => f.Url == uri) ?? new SkriptFile(uri);
        }
        
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