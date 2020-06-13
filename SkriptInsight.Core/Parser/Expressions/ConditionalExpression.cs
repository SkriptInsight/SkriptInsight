using System.Collections.Generic;
using JetBrains.Annotations;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Parser.Types;
using SkriptInsight.Core.Parser.Types.Impl.Generic;
using SkriptInsight.Core.Parser.Types.Impl.Internal;
using SkriptInsight.Core.SyntaxInfo;

namespace SkriptInsight.Core.Parser.Expressions
{
    public class ConditionalExpression : IExpression
    {
        public ConditionalExpression(SkriptCondition condition, int patternIndex, Range range, ParseContext context)
        {
            Condition = condition;
            PatternIndex = patternIndex;
            Range = range;
            Context = context;
            Type = InternalSkriptConditionType.Instance;
        }
        
        [CanBeNull] public SkriptCondition Condition { get; set; }

        public int PatternIndex { get; set; }

        public object Value
        {
            get => Condition;
            set => Condition = value as SkriptCondition;
        }

        public ISkriptType Type { get; set; }
        public Range Range { get; set; }
        public ParseContext Context { get; set; }

        public string AsString()
        {
            throw new System.NotImplementedException();
        }

        public List<MatchAnnotation> MatchAnnotations { get; set; } = new List<MatchAnnotation>();
    }
}