using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SkriptInsight.Core.SyntaxInfo
{
    public class SkriptAddonDocumentation
    {
        public SkriptAddon Addon { get; set; }

        public List<SkriptType> Types { get; set; }

        public List<SkriptCondition> Conditions { get; set; }

        public List<SkriptEvent> Events { get; set; }

        [JsonProperty("expressions")]
        protected List<SkriptExpression> InnerExpressions { get; set; }
        
        public List<SkriptExpression> Expressions { get; } = new List<SkriptExpression>();

        public List<SkriptEffect> Effects { get; set; }
        
        private readonly int[] _expressionTypesStartIndices = new int[Enum.GetValues(typeof(ExpressionType)).Length];

        public void LoadPatterns()
        {
            var types = Enum.GetValues(typeof(ExpressionType)).Cast<ExpressionType>().ToList();
            foreach (var expr in InnerExpressions)
            {
                var typeOrdinal = (int)expr.ExpressionType;
                for (var i = typeOrdinal + 1; i < types.Count; i++) {
                    _expressionTypesStartIndices[i]++;
                }
                Expressions.Insert(_expressionTypesStartIndices[typeOrdinal], expr);

            }
        }
    }
}