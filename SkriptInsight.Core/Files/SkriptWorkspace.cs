using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using SkriptInsight.Core.Managers;
using SkriptInsight.Core.SyntaxInfo;

namespace SkriptInsight.Core.Files
{
    /// <summary>
    /// Represents a Skript workspace
    /// </summary>
    public class SkriptWorkspace
    {
        public ISkriptInsightHost Host => WorkspaceManager.CurrentHost;

        public SkriptWorkspace()
        {
            LoadAddons();
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

        public List<SkriptFile> Files { get; set; } = new List<SkriptFile>();
    }
}