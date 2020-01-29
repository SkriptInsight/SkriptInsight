using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SkriptInsight.Core.Parser.Patterns;
using SkriptInsight.Core.Parser.Patterns.Impl;
using SkriptInsight.JavaReader;

namespace SkriptInsight.Core.SyntaxInfo
{
    [DebuggerDisplay("{" + nameof(NamePattern) + "} | {Parent.Name}")]
    public class SkriptEventValueExpression : SkriptExpression
    {
        private JavaClass _returnType;
        public SkriptEvent Parent { get; }

        public SkriptPattern NamePattern { get; set; }

        public JavaClass JavaReturnType => _returnType ??= LoadedClassRepository.Instance[ReturnType];

        public string RawName { get; set; }

        public sealed override string ReturnType { get; set; }

        public SkriptEventValueExpression(EventValueInfo valueInfo, SkriptEvent parent)
        {
            Parent = parent;
            RawName = valueInfo.ValueName;
            InitializeNamePattern(valueInfo);
           
            PatternNodes = new[] {NamePattern};
            ReturnType = valueInfo.ValueClass;
        }

        private void InitializeNamePattern(EventValueInfo valueInfo)
        {
            NamePattern = SkriptPattern.ParsePattern("[the ]" + valueInfo.ValueName);
        }
    }
}