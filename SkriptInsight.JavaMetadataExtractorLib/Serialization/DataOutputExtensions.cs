using System;
using Apache.NBCEL.Java.IO;
using SkriptInsight.JavaMetadata.Model;
using SkriptInsight.JavaReader;
using static SkriptInsight.JavaMetadataExtractorLib.MetadataIo;

namespace SkriptInsight.JavaMetadataExtractorLib.Serialization
{
    public static class DataOutputExtensions
    {
        public static void WriteJarArchive(this DataOutputStream stream, MetadataJarArchive archive)
        {
            LogVerbose($"Writing {archive.JavaClasses.Count} metadata classes to file.");
            stream.WriteInt(archive.JavaClasses.Count);
            foreach (var (key, value) in archive.JavaClasses)
            {
                LogDebug($"Writing the class {key} to file.");
                stream.WriteJarArchiveClass(key, value);
            }

            LogVerbose($"Finished writing {archive.JavaClasses.Count} metadata classes to file.");
        }

        private static void WriteJarArchiveClass(this DataOutputStream stream, string name, MetadataJavaClass @class)
        {
            stream.WriteUTF(name);
            stream.WriteJarClass(@class);
        }

        private static void WriteJarClass(this DataOutputStream stream, MetadataJavaClass @class)
        {
            stream.WriteUTF(@class.ClassName);
            stream.WriteAccessFlagsParameter(@class.AccessFlags);
            stream.WriteArray(@class.Interfaces, stream.WriteUTF);
            stream.WriteArray(@class.AllInterfaces, stream.WriteUTF);
            stream.WriteArray(@class.Interfaces, stream.WriteUTF);
            stream.WriteArray(@class.AllSuperClasses, stream.WriteUTF);
            stream.WriteArray(@class.Methods, stream.WriteJavaMethod);
            stream.WriteArray(@class.Fields, stream.WriteJavaField);
        }

        private static void WriteJavaMethod(this DataOutputStream stream, MetadataJavaMethod method)
        {
            stream.WriteUTF(method.Name);
            stream.WriteUTF(method.ReturnType);
            stream.WriteAccessFlagsParameter(method.AccessFlags);
            stream.WriteArray(method.Parameters, stream.WriteJavaParameter);
        }

        private static void WriteJavaField(this DataOutputStream stream, MetadataJavaField field)
        {
            stream.WriteUTF(field.Name);
            stream.WriteUTF(field.Type);
            stream.WriteAccessFlagsParameter(field.AccessFlags);
            stream.WriteBoolean(field.ConstantValue != null);
            if (field.ConstantValue != null)
                stream.WriteArray(field.ConstantValue, b => stream.WriteByte(b));
        }

        private static void WriteAccessFlagsParameter(this DataOutputStream stream, JavaAccessFlags flags)
        {
            stream.WriteInt((int) flags);
        }

        private static void WriteJavaParameter(this DataOutputStream stream, MetadataJavaMethodParameter parameter)
        {
            stream.WriteUTF(parameter.Name);
            stream.WriteUTF(parameter.Type);
        }

        private static void WriteArray<T>(this DataOutputStream stream, T[] array, Action<T> writer)
        {
            stream.WriteInt(array.Length);
            foreach (var value in array)
            {
                writer(value);
            }
        }
    }
}