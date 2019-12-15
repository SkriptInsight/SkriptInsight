using System.Collections.Generic;
using System.Linq;
using SkriptInsight.Core.Managers;
using SkriptInsight.Core.Parser.Expressions;
using SkriptInsight.Core.Parser.Patterns.Impl;

namespace SkriptInsight.Core.Parser.Types.Impl.Generic
{
    [TypeDescription("object")]
    public class GenericSkriptObject : SkriptGenericType<GenericSkriptObject.WrappedObject>
    {
        public class WrappedObject
        {
            public IExpression Expression { get; }
            
            public override string ToString()
            {
                return Type?.AsString(Expression) ?? "<none>";
            }

            public ISkriptType Type { get; }

            public WrappedObject(IExpression expression, ISkriptType type)
            {
                Expression = expression;
                Type = type;
            }
        }
        
        protected override WrappedObject TryParse(ParseContext ctx)
        {
            var clone = ctx.Clone();
            var typePattern = new TypePatternElement();
            var possibleValues = new List<(int lastPos, ISkriptType type, IExpression result)>();

            var startPos = clone.CurrentPosition;
            foreach (var type in WorkspaceManager.CurrentWorkspace.KnownTypesFromAddons)
            {
                if (type.IsPlural) continue;
                clone.Matches.Clear();
                clone.CurrentPosition = startPos;
                typePattern.Type = type.FinalTypeName;
                var result = typePattern.Parse(clone);

                if (result.IsSuccess)
                {
                    var expression = clone.Matches.OfType<ExpressionParseMatch>().FirstOrDefault();
                    if (expression?.Expression != null || expression?.Expression?.Value != null )
                    {
                        possibleValues.Add((clone.CurrentPosition, expression?.Expression.Type, expression.Expression));
                    }
                }
            }

            possibleValues.Sort((c1, c2) => -1 * c1.lastPos.CompareTo(c2.lastPos));

            if (possibleValues.Count <= 0) return null;
            
            {
                var (lastPos, type, result) = possibleValues[0];
                
                ctx.ReadUntilPosition(lastPos);
                return new WrappedObject(result, type);
            }
        }

        public override string AsString(WrappedObject obj)
        {
            return obj.ToString();
        }
    }
}