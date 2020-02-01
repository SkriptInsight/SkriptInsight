using System.Diagnostics;
using System.Text.Json.Serialization;
using SkriptInsight.Core.Parser.Patterns;
using SkriptInsight.JavaReader;

namespace SkriptInsight.Core.SyntaxInfo
{
    [DebuggerDisplay("{" + nameof(NamePattern) + "} | {Parent.Name}")]
    public class SyntaxSkriptEventValueExpression : SyntaxSkriptExpression
    {
        private JavaClass _returnType;
        public SkriptEvent Parent { get; }
        
        public override string ClassName => ReturnType;
        
        public SkriptPattern NamePattern { get; set; }

        [JsonIgnore]
        public JavaClass JavaReturnType => _returnType ??= LoadedClassRepository.Instance[ReturnType];

        public string RawName { get; set; }

        public sealed override string ReturnType { get; set; }

        public SyntaxSkriptEventValueExpression(EventValueInfo valueInfo, SkriptEvent parent)
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