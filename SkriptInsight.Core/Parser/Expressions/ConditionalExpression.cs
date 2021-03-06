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
        public ConditionalExpression(SkriptCondition condition, List<ParseMatch> matches, int patternIndex, Range range,
            ParseContext context)
        {
            Condition = condition;
            Matches = matches;
            PatternIndex = patternIndex;
            Range = range;
            Context = context;
            Type = InternalSkriptConditionType.Instance;
            RemoveMatchElementFromMatches();
        }

        private void RemoveMatchElementFromMatches()
        {
            foreach (var it in Matches)
            {
                if (it is ExpressionParseMatch expressionParseMatch)
                {
                    expressionParseMatch.MatchedElement = null;
                }
            }
        }

        [CanBeNull] public SkriptCondition Condition { get; set; }

        public List<ParseMatch> Matches { get; set; }

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