using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Apache.NBCEL;
using Newtonsoft.Json;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Managers;
using SkriptInsight.Core.SyntaxInfo;
using SkriptInsight.JavaMetadataExtractorLib;
using SkriptInsight.JavaMetadataExtractorLib.MetadataRepresentation;
using SkriptInsight.JavaReader;

namespace SkriptInsight.Core.Files
{
    /// <summary>
    /// Represents a Skript workspace
    /// </summary>
    public class SkriptWorkspace
    {
        public WorkspaceManager WorkspaceManager { get; }

        public ISkriptInsightHost Host => WorkspaceManager.CurrentHost;

        public SkriptTypesManager TypesManager { get; set; }
        
        public SkriptWorkspace(WorkspaceManager manager = null)
        {
            WorkspaceManager = manager ?? WorkspaceManager.Instance;
            TypesManager = new SkriptTypesManager();
        }

        internal void InitWorkspace()
        {
            LoadAddons();
            LoadJarMetadata();
        }

        private void LoadJarMetadata()
        {
            var knownMetadata = new[]
            {
                "rt", //Base Java classes
                "spigot-api-1.14.4", //Spigot API classes
                "Skript" //Skript classes
            };
            
            var archives = new List<JarArchive>();
            foreach (var metaName in knownMetadata)
            {
                var assembly = typeof(SkriptWorkspace).Assembly;
                var resourceName = assembly.GetManifestResourceNames()
                    .SingleOrDefault(str => str.EndsWith($"{metaName}.simeta"));
                if (resourceName == null) continue;
                
                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null) continue;
                
                //Load metadata
                archives.Add(MetadataIo.ReadArchiveMetadataFromStream(stream).ToDataClass());
            }
            archives.ForEach(a => a.LoadDataProperties());
            
            GC.Collect();
        }

        private void LoadAddons()
        {
            var knownAddons = new[]
            {
                "Skript"
            };

            foreach (var addon in knownAddons)
            {
                var assembly = typeof(SkriptWorkspace).Assembly;
                var resourceName = assembly.GetManifestResourceNames()
                    .Single(str => str.EndsWith($"{addon}.json"));

                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null) continue;

                using var reader = new StreamReader(stream);
                var result = reader.ReadToEnd();

                var doc = JsonConvert.DeserializeObject<SkriptAddonDocumentation>(result);
                doc.LoadPatterns();

                //Take all singular types and make them plural
                doc.Types.AddRange(doc.Types.ToList().Where(t => !t.IsPlural).Select(t =>
                {
                    var clone = t.Clone();
                    clone.IsPlural = true;
                    return clone;
                }));
                //Sort the types by their position on the list and plural version first

                doc.Types = doc.Types
                    .GroupBy(c => c.Id)
                    .SelectMany(c => c.OrderByDescending(cc => cc.IsPlural ? 1 : 0))
                    .ToList();

                AddonDocumentations.Add(doc);
            }
        }

        public List<SkriptAddonDocumentation> AddonDocumentations { get; set; } = new List<SkriptAddonDocumentation>();

        public ConcurrentFileDictionary Files { get; set; } = new ConcurrentFileDictionary();
    }
}