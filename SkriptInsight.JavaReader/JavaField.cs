using System.Diagnostics;
using Apache.NBCEL.ClassFile;
using Apache.NBCEL.Generic;
using AccessFlags = SkriptInsight.JavaReader.AccessFlags;

namespace SkriptInsight.JavaReader
{
    public class JavaField
    {
        public JavaField()
        {
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private AccessFlags? _flags;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected Field InternalField { get; }

        public virtual ConstantPool ConstantPool => InternalField.GetConstantPool();

        public virtual AccessFlags Flags => _flags ??= (AccessFlags) InternalField.GetAccessFlags();

        public virtual string Name => InternalField.GetName();

        public virtual Type Type => InternalField.GetType();

        public virtual Constant ConstantValue =>
            InternalField.GetConstantValue() != null
                ? InternalField.GetConstantPool().GetConstant(InternalField.GetConstantValue().GetConstantValueIndex())
                : null;

        public JavaField(Field internalField)
        {
            InternalField = internalField;
        }

        public override string ToString()
        {
            return InternalField.ToString();
        }
    }
}