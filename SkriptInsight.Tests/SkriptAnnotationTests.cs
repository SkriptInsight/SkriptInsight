using System.Diagnostics;
using SkriptInsight.Core.Annotations;
using SkriptInsight.Core.Annotations.Impl;
using SkriptInsight.Core.Annotations.Parameters;
using Xunit;

namespace SkriptInsight.Tests
{
    public class SkriptAnnotationTests
    {
        [AnnotationAlias("empty")]
        class EmptyAnnotation : Annotation
        {
        }

        [AnnotationAlias("onestring", "1str")]
        class OneStringAnnotation : Annotation
        {
            [Parameter]
            public string Value { get; set; }
        }

        [Fact]
        public void EmptyAnnotationParserWorks()
        {
            var code = "@empty";
            var parser = new AnnotationParser();
            parser.RegisterAnnotation<EmptyAnnotation>();

            var annotation = parser.TryParse(code);
            Assert.NotNull(annotation);
            Assert.IsType<EmptyAnnotation>(annotation);

            parser.UnregisterAnnotation<EmptyAnnotation>();
        }

        [Fact]
        public void OneStringParserWorks()
        {
            var code = new[] {"@onestring ", "@1str "};
            var args = "one argument parameter";
            var parser = new AnnotationParser();
            parser.RegisterAnnotation<OneStringAnnotation>();

            foreach (var prefix in code)
            {
                var annotation = parser.TryParse(prefix + args);
                Assert.NotNull(annotation);
                Assert.IsType<OneStringAnnotation>(annotation);
                Assert.Equal(args, ((OneStringAnnotation) annotation).Value);
            }


            parser.UnregisterAnnotation<EmptyAnnotation>();
        }
    }
}