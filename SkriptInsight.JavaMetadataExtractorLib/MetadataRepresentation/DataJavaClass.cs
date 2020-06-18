using System;
using System.Linq;
using SkriptInsight.JavaMetadata;
using SkriptInsight.JavaMetadata.Model;
using SkriptInsight.JavaReader;

namespace SkriptInsight.JavaMetadataExtractorLib.MetadataRepresentation
{
    public sealed class DataJavaClass : JavaClass
    {
        private JavaClass[] _metaSuperClasses;
        private JavaClass[] _metaInterfaces;
        private JavaClass[] _metaAllInterfaces;
        private DataJavaField[] _metaFields;
        private DataJavaMethod[] _metaMethods;
        private AccessFlags _metaFlags;

        public DataJavaClass(MetadataJavaClass meta)
        {
            FullClassName = meta.ClassName;
            CanLoad += (_, __) => PrepareProperties(meta);
        }

        public void PrepareProperties(MetadataJavaClass meta)
        {
            ClassName = meta.ClassName.Substring(meta.ClassName.LastIndexOf('.') + 1);
            PackageName = meta.ClassName.Substring(0, meta.ClassName.LastIndexOf('.'));
            _metaFlags = (AccessFlags) meta.AccessFlags;
            _metaSuperClasses ??= meta.AllSuperClasses.Select(c => LoadedClassRepository.Instance[c]).ToArray();
            _metaInterfaces ??= meta.Interfaces.Select(c => LoadedClassRepository.Instance[c]).ToArray();
            _metaAllInterfaces ??= meta.AllInterfaces.Select(c => LoadedClassRepository.Instance[c]).ToArray();
            _metaFields ??= meta.Fields.Select(c => c.ToDataClass()).ToArray();
            _metaMethods ??= meta.Methods.Select(c => c.ToDataClass()).ToArray();
        }

        public override string ClassName { get; set; }

        public override string FullClassName { get; set; }

        public override string PackageName { get; set; }

        public override AccessFlags Flags => _metaFlags;

        public override JavaClass[] SuperClasses => _metaSuperClasses;

        public override JavaClass[] Interfaces => _metaInterfaces;

        public override JavaClass[] AllInterfaces => _metaAllInterfaces;

        public override JavaField[] Fields => _metaFields;

        public override JavaMethod[] Methods => _metaMethods;

        public override string ToString()
        {
            return FullClassName;
        }

        public event EventHandler<EventArgs> CanLoad;

        internal void OnCanLoad()
        {
            CanLoad?.Invoke(this, EventArgs.Empty);
            foreach (var f in SuperClasses)
                if (f is DataJavaClass javaClass)
                    javaClass.OnCanLoad();

            foreach (var f in Interfaces)
                if (f is DataJavaClass javaInterface)
                    javaInterface.OnCanLoad();

            foreach (var f in AllInterfaces)
                if (f is DataJavaClass javaInterface)
                    javaInterface.OnCanLoad();

            foreach (var f in Methods)
                if (f is DataJavaMethod javaMethod)
                    javaMethod.OnCanLoad();

            foreach (var f in Fields)
                if (f is DataJavaField javaField)
                    javaField.OnCanLoad();

            CanLoad = null;
        }
    }
}