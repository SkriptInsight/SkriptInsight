using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using MoreLinq.Extensions;
using SkriptInsight.Core.Annotations.Parameters;
using SkriptInsight.Core.Annotations.Parameters.Readers;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Parser;
using SkriptInsight.Core.Parser.Patterns;
using SkriptInsight.Core.Parser.Patterns.Impl;

namespace SkriptInsight.Core.Annotations
{
    public class AnnotationParser
    {
        [CanBeNull] private static AnnotationParser _instance;

        public AnnotationParser()
        {
            RegisterParameterReader(new StringReader());
            RegisterParameterReader(new TypeReader());
        }

        private Dictionary<Type, IParameterReader> TypeReaders { get; } = new Dictionary<Type, IParameterReader>();

        private Dictionary<string, AnnotationDescription> Annotations { get; } =
            new Dictionary<string, AnnotationDescription>();

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

        #region Annotation Registry

        public void RegisterAnnotation<T>() where T : Annotation
        {
            var aliases = typeof(T).GetCustomAttribute<AnnotationAliasAttribute>()?.Aliases;
            aliases?.ForEach(alias =>
                Annotations[alias] =
                    new AnnotationDescription(this, typeof(T),
                        () => (Annotation) FormatterServices.GetUninitializedObject(typeof(T))));
        }

        public void UnregisterAnnotation<T>() where T : Annotation
        {
            var aliases = typeof(T).GetCustomAttribute<AnnotationAliasAttribute>()?.Aliases;
            aliases?.ForEach(alias => Annotations.Remove(alias));
        }

        #endregion


        private SkriptPattern _annotationPattern;
        [NotNull] private SkriptPattern AnnotationPattern => _annotationPattern ??= ComputeAnnotationPattern();

        private SkriptPattern ComputeAnnotationPattern()
        {
            var pattern = new SkriptPattern();
            pattern.Children.Add(new LiteralPatternElement("@"));
            var annotationsName = new ChoicePatternElement {SaveChoice = true};

            foreach (var key in Annotations.Keys)
            {
                annotationsName.Elements.Add(
                    new ChoicePatternElement.ChoiceGroupElement(new LiteralPatternElement(key)));
            }

            pattern.Children.Add(annotationsName);

            return pattern;
        }

        [CanBeNull]
        public Annotation TryParse(string text)
        {
            var result = AnnotationPattern.Parse(text);
            if (!result.IsSuccess) return null;

            var alias = result.Matches.ElementAtOrDefault(0)?.RawContent;
            if (alias == null) return null;

            var ann = Annotations
                .GetValueOrDefault(alias,
                    new AnnotationDescription(this, typeof(UnknownAnnotation), () => new UnknownAnnotation(alias)));

            var (parameters,  annotation) = (ann.Parameters, ann.Initializer.Invoke());
            
            var args = new Stack<string>(text.Split(" ").Skip(1).Reverse());
            for (var index = 0; index < parameters.Length; index++)
            {
                var parameter = parameters[index];
                var context = new ParameterContext(index, index == parameters.Length - 1);
                var parameterValue = parameter.ParameterReader?.TryParse(args, context) ?? throw new InvalidOperationException($"Unable to find a type parser for parameter {parameter.Property.Name} on {parameter.Property.DeclaringType?.FullName}.");
                try
                {
                    parameter.Property.SetValue(annotation, parameterValue);
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException(
                        $"Unable to set parameter {parameter.Property.Name} on {parameter.Property.DeclaringType?.FullName}.", e);
                }
            }

            return annotation;
        }

        public IParameterReader GetReaderForType(Type type)
        {
            return TypeReaders[type];
        }
    }
}