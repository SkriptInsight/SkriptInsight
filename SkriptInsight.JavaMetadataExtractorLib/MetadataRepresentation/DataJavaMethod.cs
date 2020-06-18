using System;
using System.Linq;
using SkriptInsight.JavaMetadata.Model;
using SkriptInsight.JavaReader;
using Type = Apache.NBCEL.Generic.Type;

namespace SkriptInsight.JavaMetadataExtractorLib.MetadataRepresentation
{
    public class DataJavaMethod : JavaMethod
    {
        private DataJavaMethodParameter[] _metaParams;
        private AccessFlags _metaFlags;
        private string _metaName;
        private Type _metaType;

        public DataJavaMethod(MetadataJavaMethod meta)
        {
            CanLoad += (_, __) => SetupProperties(meta);
        }

        private void SetupProperties(MetadataJavaMethod meta)
        {
            _metaFlags = (AccessFlags) meta.AccessFlags;
            _metaName = meta.Name;
            _metaType = Type.GetType(meta.ReturnType);
            _metaParams = meta.Parameters.Select(c => c.ToDataClass()).ToArray();
        }

        public override AccessFlags Flags => _metaFlags;

        public override string Name => _metaName;

        public override Type Type => _metaType;

        public override JavaMethodParameter[] Parameters => _metaParams;

        public override string ToString()
        {
            return Name;
        }

        public event EventHandler<EventArgs> CanLoad;

        internal void OnCanLoad()
        {
            CanLoad?.Invoke(this, EventArgs.Empty);
            foreach (var parameter in Parameters)
            {
                if (parameter is DataJavaMethodParameter javaMethodParameter)
                    javaMethodParameter.OnCanLoad();
            }

            CanLoad = null;
        }
    }
}