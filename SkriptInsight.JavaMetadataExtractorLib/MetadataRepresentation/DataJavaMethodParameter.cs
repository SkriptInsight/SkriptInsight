using System;
using SkriptInsight.JavaMetadata.Model;
using SkriptInsight.JavaReader;
using Type = Apache.NBCEL.Generic.Type;

namespace SkriptInsight.JavaMetadataExtractorLib.MetadataRepresentation
{
    public class DataJavaMethodParameter : JavaMethodParameter
    {
        private Type _metaType;
        private string _metaName;

        public DataJavaMethodParameter(MetadataJavaMethodParameter meta)
        {
            CanLoad += (_, __) => SetupProperties(meta);
        }

        private void SetupProperties(MetadataJavaMethodParameter Meta)
        {
            _metaName = Meta.Name;
            _metaType = Type.GetType(Meta.Type);
        }

        public override string Name => _metaName;

        public override Type Type => _metaType;

        public event EventHandler<EventArgs> CanLoad;

        internal void OnCanLoad()
        {
            CanLoad?.Invoke(this, EventArgs.Empty);
            CanLoad = null;
        }
    }
}