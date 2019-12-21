using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Files;
using SkriptInsight.Core.SyntaxInfo;

namespace SkriptInsight.Core.Managers
{
    public class WorkspaceManager
    {
        private WorkspaceManager()
        {
            Current = new SkriptWorkspace(this);
            KnownTypesManager = new KnownTypesManager(this);
            //First of all, init the workspace
            Current.InitWorkspace();
            
            //Init the types
            Current.TypesManager.InitTypesFromAddons(Current, this);
            //Then finally load them (from cache)
            KnownTypesManager.LoadTypes();
            
            //Load the expressions at the end
            Current.TypesManager.LoadExpressionsFromTypes();
        }

        public static ISkriptInsightHost CurrentHost { get; set; }
        
        public static WorkspaceManager Instance { get; } = new WorkspaceManager();
        
        public KnownTypesManager KnownTypesManager { get; }
        
        public SkriptWorkspace Current { get; }
        
        public static SkriptWorkspace CurrentWorkspace => Instance.Current;

        public SkriptFile GetOrCreateByUri(Uri uri)
        {
            return Current.Files.GetOrAdd(uri, u => new SkriptFile(u));
        }
        
        public void HandleOpenedFile(SkriptFile file)
        {
            if (!Current.Files.ContainsKey(file.Url))
                Current.Files[file.Url] = file;
        }

        public void HandleClosedFile(SkriptFile file)
        {
            if (Current.Files.ContainsKey(file.Url))
                Current.Files.TryRemove(file.Url, out _);
        }
    }
}