using System.IO;
using System.Linq;
using Apache.NBCEL.ClassFile;
using Apache.NBCEL.Generic;
using Apache.NBCEL.Java.IO;
using Apache.NBCEL.Java.Nio;
using SkriptInsight.JavaMetadata;
using SkriptInsight.JavaMetadata.Model;
using SkriptInsight.JavaReader;
using JavaClass = SkriptInsight.JavaReader.JavaClass;

namespace SkriptInsight.JavaMetadataExtractorLib
{
    public static class Extensions
    {
        public static MetadataJavaField ToMetadata(this JavaField field)
        {
            var javaField = new MetadataJavaField
            {
                Name = field.Name,
                Type = field.Type.GetSignature()
            };

            if (field.ConstantValue != null)
            {
                using var stream = new MemoryStream();
                using var ms = new MemoryOutputStream(stream);
                using var dos = new DataOutputStream(ms);

                field.ConstantValue.Dump(dos);

                //Write the original string there to be able to create a new constantpool from metadata
                if (field.ConstantValue is ConstantString constantString)
                    dos.WriteUTF(constantString.GetBytes(field.ConstantPool));

                javaField.ConstantValue = stream.ToArray();
            }

            return javaField;
        }

        public static MetadataJavaMethodParameter ToMetadata(this JavaMethodParameter parameter)
        {
            return new MetadataJavaMethodParameter
            {
                Name = parameter.Name,
                Type = parameter.Type.GetSignature()
            };
        }

        public static MetadataJavaMethod ToMetadata(this JavaMethod method)
        {
            return new MetadataJavaMethod
            {
                AccessFlags = (JavaAccessFlags) method.Flags,
                ReturnType = method.Type.GetSignature(),
                Name = method.Name,
                Parameters = method.Parameters.Select(p => p.ToMetadata()).ToArray()
            };
        }

        public static MetadataJavaClass ToMetadata(this JavaClass javaClass)
        {
            return new MetadataJavaClass
            {
                AccessFlags = (JavaAccessFlags) javaClass.Flags,
                ClassName = javaClass.FullClassName,
                AllSuperClasses = javaClass.SuperClasses.Select(c => c.FullClassName).ToArray(),
                Interfaces = javaClass.Interfaces.Select(c => c.FullClassName).ToArray(),
                AllInterfaces = javaClass.Interfaces.Select(c => c.FullClassName).ToArray(),
                Methods = javaClass.Methods.Select(m => m.ToMetadata()).ToArray(),
                Fields = javaClass.Fields.Select(f => f.ToMetadata()).ToArray()
            };
        }

        public static MetadataJarArchive ToMetadata(this JarArchive archive)
        {
            return new MetadataJarArchive
            {
                JavaClasses = archive.Classes.Select(c => (c.Key, c.Value.ToMetadata()))
                    .ToDictionary(c => c.Item1, c => c.Item2)
            };
        }
    }
}