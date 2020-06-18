using System.Diagnostics;
using Apache.NBCEL.ClassFile;
using Apache.NBCEL.Generic;
using AccessFlags = SkriptInsight.JavaReader.AccessFlags;

namespace SkriptInsight.JavaReader
{
    public class JavaMethod
    {
        public JavaMethod()
        {
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private AccessFlags? _flags;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private JavaMethodParameter[] _params;

        public JavaMethod(Method method)
        {
            InternalMethod = method;
        }

        private Method InternalMethod { get; }

        public virtual AccessFlags Flags => _flags ??= (AccessFlags) InternalMethod.GetAccessFlags();

        public virtual string Name => InternalMethod.GetName();

        public virtual Type Type => InternalMethod.GetReturnType();

        public virtual JavaMethodParameter[] Parameters => _params ??= JavaMethodParameter.FromMethod(InternalMethod);

        public override string ToString()
        {
            return InternalMethod.ToString();
        }
    }
}