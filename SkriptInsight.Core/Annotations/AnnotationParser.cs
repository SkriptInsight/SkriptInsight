using System;
using System.Collections.Generic;
using SkriptInsight.Core.Annotations.Parameters;
using SkriptInsight.Core.Annotations.Parameters.Readers;

namespace SkriptInsight.Core.Annotations
{
    public class AnnotationParser
    {
        private static AnnotationParser? _instance;
        public static AnnotationParser Instance => _instance ??= new AnnotationParser();

        private Dictionary<Type, IParameterReader> TypeReaders { get; set; } = new Dictionary<Type, IParameterReader>();

        #region Registry

        public void RegisterInlineGenericParameterReader<T>(Type type, Func<Stack<string>, T> reader)
        {
            RegisterParameterReader(type, new DelegateReader(stack => reader(stack)));
        }

        public void RegisterInlineParameterReader(Type type, Func<Stack<string>, object?> reader)
        {
            RegisterParameterReader(type, new DelegateReader(reader));
        }

        public void RegisterParameterReader<T>(GenericParameterReader<T> reader)
        {
            RegisterParameterReader(typeof(T), reader);
        }

        public void RegisterParameterReader(Type type, IParameterReader reader)
        {
            TypeReaders[type] = reader;
        }

        #endregion
    }
}