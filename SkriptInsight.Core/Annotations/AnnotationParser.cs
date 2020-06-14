using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SkriptInsight.Core.Annotations.Parameters;
using SkriptInsight.Core.Annotations.Parameters.Readers;

namespace SkriptInsight.Core.Annotations
{
    public class AnnotationParser
    {
        [CanBeNull] private static AnnotationParser _instance;

        public AnnotationParser()
        {
            RegisterParameterReader(new StringReader());
        }

        public static AnnotationParser Instance => _instance ??= new AnnotationParser();

        private Dictionary<Type, IParameterReader> TypeReaders { get; } = new Dictionary<Type, IParameterReader>();

        #region Registry

        public void RegisterInlineGenericParameterReader<T>(Type type, Func<Stack<string>, ParameterContext, T> reader)
        {
            RegisterParameterReader(type, new DelegateReader((stack, context) => reader(stack, context)));
        }

        public void RegisterInlineParameterReader(Type type, Func<Stack<string>, ParameterContext, object> reader)
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