using System;
using Apache.NBCEL.Java.IO;
using SkriptInsight.JavaMetadata.Model;

namespace SkriptInsight.JavaMetadataExtractorLib.Serialization
{
    public static class DataInputExtensions
    {
        public static string ReadUtfAndIntern(this DataInputStream stream)
        { 
            return string.Intern(stream.ReadUTF());
        }
        public static MetadataJarArchive ReadJarArchive(this DataInputStream stream)
        {
            var archive = new MetadataJarArchive();
            var length = stream.ReadInt();
            for (var i = 0; i < length; i++)
            {
                var (key, value) = stream.ReadJarArchiveClass();
                archive.JavaClasses[key] = value;
            }

            return archive;
        }

        private static (string, MetadataJavaClass) ReadJarArchiveClass(this DataInputStream stream)
        {
            var name = stream.ReadUtfAndIntern();
            var @class = stream.ReadJarClass();
            return (name, @class);
        }

        private static MetadataJavaClass ReadJarClass(this DataInputStream stream)
        {
            var @class = new MetadataJavaClass
            {
                ClassName = stream.ReadUtfAndIntern(),
                Interfaces = stream.ReadArray(stream.ReadUtfAndIntern),
                AllInterfaces = stream.ReadArray(stream.ReadUtfAndIntern)
            };
            @class.Interfaces = stream.ReadArray(stream.ReadUtfAndIntern);
            @class.AllSuperClasses = stream.ReadArray(stream.ReadUtfAndIntern);
            @class.Methods = stream.ReadArray(stream.ReadJavaMethod);
            @class.Fields = stream.ReadArray(stream.ReadJavaField);

            return @class;
        }

        private static MetadataJavaMethod ReadJavaMethod(this DataInputStream stream)
        {
            var method = new MetadataJavaMethod
            {
                Name = stream.ReadUtfAndIntern(),
                ReturnType = stream.ReadUtfAndIntern(),
                AccessFlags = stream.ReadAccessFlagsParameter(),
                Parameters = stream.ReadArray(stream.ReadJavaParameter)
            };
            return method;
        }

        private static MetadataJavaField ReadJavaField(this DataInputStream stream)
        {
            var field = new MetadataJavaField
            {
                Name = stream.ReadUtfAndIntern(), Type = stream.ReadUtfAndIntern(), AccessFlags = stream.ReadAccessFlagsParameter()
            };

            var hasConstantValue = stream.ReadBoolean();
            if (hasConstantValue)
            {
                field.ConstantValue = stream.ReadArray(stream.ReadByte);
            }

            return field;
        }

        private static JavaAccessFlags ReadAccessFlagsParameter(this DataInputStream stream)
        {
            return (JavaAccessFlags) stream.ReadInt();
        }

        private static MetadataJavaMethodParameter ReadJavaParameter(this DataInputStream stream)
        {
            return new MetadataJavaMethodParameter {Name = stream.ReadUtfAndIntern(), Type = stream.ReadUtfAndIntern()};
        }

        private static T[] ReadArray<T>(this DataInputStream stream, Func<T> reader)
        {
            var length = stream.ReadInt();
            
            
            var returningArray = new T[length];
            for (var i = 0; i < length; i++)
            {
                    returningArray[i] = reader.Invoke();
            }

            return returningArray;
        }
    }
}