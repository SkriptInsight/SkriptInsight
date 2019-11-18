using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
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