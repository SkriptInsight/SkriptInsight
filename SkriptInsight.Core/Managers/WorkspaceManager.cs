using System;
using System.IO;
using System.Linq;
using SkriptInsight.Core.Files;
using SkriptInsight.Core.Files.Nullability;

namespace SkriptInsight.Core.Managers
{
    public class WorkspaceManager
    {
        private const string MetadataFileExtension = ".skm";

        public static WorkspaceManager Instance { get; } = new WorkspaceManager();

        public SkriptWorkspace Current { get; } = new SkriptWorkspace();

        public static SkriptWorkspace CurrentWorkspace => Instance.Current;

        public SkriptFile GetOrCreateByUri(Uri uri)
        {
            var extension = Path.GetExtension(uri.AbsolutePath);
            
            return Current.Files.FirstOrDefault(f => f.Url == uri) ?? (extension == MetadataFileExtension
                       ? new MetadataSkriptFile(uri)
                       : new SkriptFile(uri));
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