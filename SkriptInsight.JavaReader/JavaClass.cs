using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SkriptInsight.JavaReader
{
    [DebuggerDisplay("{FullClassName}")]
    public class JavaClass
    {
        public JavaClass()
        {
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private AccessFlags? _accessFlags;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private JavaClass[] _superClasses;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private JavaClass[] _interfaces;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private JavaField[] _fields;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private JavaClass[] _allInterfaces;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private JavaMethod[] _methods;
        
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Apache.NBCEL.ClassFile.JavaClass InternalClass { get; }

        public JavaClass(Apache.NBCEL.ClassFile.JavaClass internalClass)
        {
            InternalClass = internalClass;
            SetupProperties(internalClass);
        }

        public virtual string ClassName { get; set; }
        
        public virtual string FullClassName { get; set; }

        public virtual string PackageName { get; set; }
        
        private void SetupProperties(Apache.NBCEL.ClassFile.JavaClass javaClass)
        {
            ClassName = javaClass.ClassName.Substring(javaClass.ClassName.LastIndexOf('.') + 1);
            PackageName = javaClass.PackageName;
            FullClassName = javaClass.ClassName;
        }

        public virtual AccessFlags Flags => _accessFlags ??= (AccessFlags) InternalClass.GetAccessFlags();

        public virtual JavaClass[] SuperClasses
            => _superClasses ??= TryOrNull(() => InternalClass.SuperClasses.Select(c => LoadedClassRepository.Instance[c.ClassName])).ToArray();

        private IEnumerable<T> TryOrNull<T>(Func<IEnumerable<T>> func)
        {
            try
            {
                return func();
            }
            catch
            {
                //ignored
                return Enumerable.Empty<T>();
            }
        }

        public virtual JavaClass[] Interfaces
            => _interfaces ??= TryOrNull(() => InternalClass.Interfaces.Select(c => LoadedClassRepository.Instance[c.ClassName])
            ).ToArray();
        
        public virtual JavaClass[] AllInterfaces
            => _allInterfaces ??= TryOrNull(() => InternalClass.AllInterfaces.Select(c => LoadedClassRepository.Instance[c.ClassName])
            ).ToArray();
        
        public virtual JavaField[] Fields
            => _fields ??= InternalClass.Fields.Select(c => new JavaField(c)).ToArray();
        
        public virtual JavaMethod[] Methods
            => _methods ??= InternalClass.Methods.Select(c => new JavaMethod(c)).ToArray();


        public virtual bool InstanceOf(JavaClass superClass)
        {
            if (Equals(superClass))
                return true;
            if (SuperClasses != null)
            {
                foreach (var superClass1 in SuperClasses)
                {
                    if (superClass1.Equals(superClass))
                        return true;
                }
            }

            return superClass.Flags.HasFlag(AccessFlags.Interface) && ImplementationOf(superClass);
        }

        public virtual bool ImplementationOf(JavaClass inter)
        {
            if (!inter.Flags.HasFlag(AccessFlags.Interface))
                return false;
            if (Equals(inter))
                return true;
            if (AllInterfaces == null) return false;
            
            foreach (var allInterface in AllInterfaces)
            {
                if (allInterface.Equals(inter))
                    return true;
            }
            return false;
        }

        protected bool Equals(JavaClass other)
        {
            return FullClassName == other.FullClassName;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((JavaClass) obj);
        }

        public override int GetHashCode()
        {
            return (FullClassName != null ? FullClassName.GetHashCode() : 0);
        }
    }
}