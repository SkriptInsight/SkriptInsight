using System;
using System.Collections.Generic;
using System.Linq;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Files.Nodes;
using SkriptInsight.Core.Utils;

namespace SkriptInsight.Core.Managers
{
    public class NodeSignaturesManager
    {
        public static NodeSignaturesManager Instance { get; } = new NodeSignaturesManager();
        
        private NodeSignaturesManager()
        {
            LoadSignatureTypes();
        }

        private void LoadSignatureTypes()
        {
            /*
             * SignatureTypes =
             * Get all loaded assemblies. Then,
             * Get all Types from them. Then,
             * Get the ones that are classes and not abstract. Then,
             * Get the ones that extend SignatureFileNode<>. Then,
             * Organize them by the base type and signature class. Then, finally,
             * Make a dictionary out of it for future lookups.
             */
            SignatureTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(c => c.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => t.IsSubclassOfRawGeneric(typeof(SignatureFileNode<>)))
                .Select(c => (BaseType: c, SignatureClass: c.BaseType?.GetGenericArguments().FirstOrDefault()))
                .Select(c => (c.BaseType,
                    TryDelegate: SignatureParserHelper.GetTryParseDelegateForType(c.SignatureClass)))
                .ToDictionary(c => c.BaseType, c => c.TryDelegate);
        }

        public Dictionary<Type, Delegate> SignatureTypes { get; set; }
    }
}