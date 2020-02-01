using System;
using Apache.NBCEL.ClassFile;
using Apache.NBCEL.Java.IO;
using Apache.NBCEL.Java.Nio;
using SkriptInsight.JavaMetadata.Model;
using SkriptInsight.JavaMetadataExtractorLib.Serialization;
using SkriptInsight.JavaReader;
using AccessFlags = SkriptInsight.JavaReader.AccessFlags;
using Type = Apache.NBCEL.Generic.Type;

namespace SkriptInsight.JavaMetadataExtractorLib.MetadataRepresentation
{
    public class DataJavaField : JavaField
    {
        private Constant _metaConstantValue;

        public DataJavaField(MetadataJavaField meta)
        {
            CanLoad += (_, __) => SetupProperties(meta);
        }

        private void SetupProperties(MetadataJavaField meta)
        {
            _metaName = meta.Name;
            _metaFlags = (AccessFlags) meta.AccessFlags;
            _metaType = Type.GetReturnType(meta.Type);
            _metaConstantValue = LoadConstantValue(meta.ConstantValue);
        }

        public override AccessFlags Flags => _metaFlags;

        public override string Name => _metaName;

        public override Type Type => _metaType;

        public override Constant ConstantValue => _metaConstantValue;

        private ConstantPool _metaConstantPool;
        
        private AccessFlags _metaFlags;
        private string _metaName;
        private Type _metaType;

        public override ConstantPool ConstantPool => _metaConstantPool;

        private Constant LoadConstantValue(byte[] metaConstantValue)
        {
            if (metaConstantValue == null) return null;
            
            using var memoryInputStream = new MemoryInputStream(metaConstantValue);
            using var ms = new DataInputStream(memoryInputStream);
            var constant = Constant.ReadConstant(ms);
            
            //Read the actual string from the data and create a constant pool for it.
            if (constant is ConstantString constantString)
            {
                var originalString = ms.ReadUtfAndIntern();
                
                _metaConstantPool = new ConstantPool(new Constant[]{new ConstantUtf8(originalString)});
                
                constantString.SetStringIndex(0);
            }
            
            return constant;
        }

        public override string ToString()
        {
            return $"{Name}";
        }

        public event EventHandler<EventArgs> CanLoad;
        internal void OnCanLoad()
        {
            CanLoad?.Invoke(this, EventArgs.Empty);
            CanLoad = null;
        }
    }
}