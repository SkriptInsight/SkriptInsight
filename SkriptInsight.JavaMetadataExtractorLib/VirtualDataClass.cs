using System.Linq;
using Apache.NBCEL.ClassFile;
using SkriptInsight.JavaMetadataExtractorLib.MetadataRepresentation;

namespace SkriptInsight.JavaMetadataExtractorLib
{
    public class VirtualDataClass : JavaClass
    {
        public override Field[] GetFields()
        {
            return new Field[0];
        }

        public override string[] GetInterfaceNames()
        {
            return _baseClass.Interfaces.Select(c => c.FullClassName).ToArray();
        }

        public override string GetSuperclassName()
        {
            return _baseClass.SuperClasses.FirstOrDefault()?.FullClassName ?? "java.lang.Object";
        }

        public override Method[] GetMethods()
        {
            return new Method[0];
        }

        private readonly JavaReader.JavaClass _baseClass;

        public VirtualDataClass(JavaReader.JavaClass baseClass)
        {
            _baseClass = baseClass;
        }

        public override string GetClassName() => _baseClass.FullClassName;

        public override string GetPackageName()
        {
            return _baseClass.PackageName;
        }
    }
}