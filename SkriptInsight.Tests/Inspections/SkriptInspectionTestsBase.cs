using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Annotations;
using SkriptInsight.Core.Files;
using SkriptInsight.Core.Managers;
using Xunit;

namespace SkriptInsight.Tests.Inspections
{
    public class SkriptInspectionTestsBase
    {
        
        public string ReadResource(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourcePath = assembly.GetManifestResourceNames()
                    .Single(str => str.EndsWith(name));

            using var stream = assembly.GetManifestResourceStream(resourcePath);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public void AssertResource(string name)
        {
            var parser = new AnnotationParser();
            parser.RegisterAnnotation<AssertInspectionAnnotation>();
            SkriptFile file = null;
            
            var host = new InspectionDelegatingHost((uri, list) =>
            {
                foreach (var diagnostic in list)
                {
                    // ReSharper disable AccessToModifiedClosure
                    Debug.Assert(file != null, nameof(file) + " != null");

                    var node = file.Nodes[diagnostic.Range.Start.Line];

                    //TODO: Allow multiple annotations to a single node by parsing the comments directly above.
                    var comment = node.RawComment.TrimStart('#').Trim();
                    if (!comment.StartsWith("@")) continue;

                    var annotation = parser.TryParse(comment);

                    switch (annotation)
                    {
                        case AssertInspectionAnnotation assertInspectionAnnotation:
                            Assert.Equal(assertInspectionAnnotation.InspectionType, diagnostic.Code);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(annotation));
                    }


                    // ReSharper restore AccessToModifiedClosure
                }
            });
            WorkspaceManager.CurrentHost = host;
            var current = WorkspaceManager.CurrentWorkspace;

            var code = ReadResource(name);

            file = new SkriptFile(new Uri($"memory://{name}"));

            file.HandleChange(new TextDocumentContentChangeEvent
            {
                Text = code
            });
            
            file.PrepareNodes();
            
            parser.UnregisterAnnotation<AssertInspectionAnnotation>();
        }
        
    }
}