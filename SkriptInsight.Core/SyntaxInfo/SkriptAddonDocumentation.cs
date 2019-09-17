using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SkriptInsight.Core.Parser.Patterns;
using SkriptInsight.Core.Parser.Patterns.Impl;

namespace SkriptInsight.Core.SyntaxInfo
{
    public class SkriptAddonDocumentation
    {
        public SkriptAddon Addon { get; set; }

        public List<SkriptType> Types { get; set; }

        public List<SkriptCondition> Conditions { get; set; }

        public List<SkriptEvent> Events { get; set; }

        [JsonProperty("expressions")] protected List<SkriptExpression> InnerExpressions { get; set; }

        public List<SkriptExpression> Expressions { get; } = new List<SkriptExpression>();

        public List<SkriptEffect> Effects { get; set; }

        private readonly int[] _expressionTypesStartIndices = new int[Enum.GetValues(typeof(ExpressionType)).Length];

        public void LoadPatterns()
        {
            Events.ForEach(e => e.LoadPatterns());
            
            var optionalOn = new OptionalPatternElement {Element = new LiteralPatternElement("on ")};
            Events.ForEach(e =>
            {
                if (e.PatternNodes.Length <= 0) return;
                foreach (var node in e.PatternNodes) node.Children.Insert(0, optionalOn);
            });
            Effects.ForEach(e => e.LoadPatterns());
            var types = Enum.GetValues(typeof(ExpressionType)).Cast<ExpressionType>().ToList();
            foreach (var expr in InnerExpressions)
            {
                var typeOrdinal = (int) expr.ExpressionType;
                for (var i = typeOrdinal + 1; i < types.Count; i++)
                {
                    _expressionTypesStartIndices[i]++;
                }

                Expressions.Insert(_expressionTypesStartIndices[typeOrdinal], expr);
            }
        }
    }
}