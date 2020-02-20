using System.Collections.Generic;
using System.Linq;
using SkriptInsight.Core.Managers;
using SkriptInsight.Core.Parser.Expressions;
using SkriptInsight.Core.Parser.Patterns;
using SkriptInsight.Core.Parser.Patterns.Impl;

namespace SkriptInsight.Core.Parser.Types.Impl.Generic
{
    [TypeDescription("object")]
    public class GenericSkriptObject : SkriptGenericType<GenericSkriptObject.WrappedObject>
    {
        public class WrappedObject
        {
            public override string ToString()
            {
                return string.Join("", Matches.Select(c => c?.ToString()));
            }

            public List<ParseMatch> Matches { get; }

            public WrappedObject(List<ParseMatch> matches)
            {
                Matches = matches;
            }
        }

        protected override WrappedObject TryParse(ParseContext ctx)
        {
            var clone = ctx.Clone();

            var typePattern = new TypePatternElement();
            var possibleValues = new List<(int CurrentPosition, List<ParseMatch> expression, ParseResult result)>();

            var startPos = clone.CurrentPosition;
            foreach (var type in WorkspaceManager.CurrentWorkspace.TypesManager.KnownTypesFromAddons)
            {
                if (type.IsPlural) continue;
                clone.Matches.Clear();
                clone.CurrentPosition = startPos;
                typePattern.Type = type.FinalTypeName;
                var result = typePattern.Parse(clone);

                if (result.IsSuccess)
                {
                    var expression = new List<ParseMatch>(clone.Matches);
                    if (expression.Count > 0)
                    {
                        possibleValues.Add((clone.CurrentPosition, expression, result));
                    }
                }
            }


            // if (possibleValues.Count <= 0)
            {
                clone.ShouldJustCheckExpressionsThatMatchType = true;
                var expressions =
                    WorkspaceManager.CurrentWorkspace.TypesManager.GetExpressionsThatCanFitType(KnownTypesManager
                        .JavaLangObjectClass);
                if (expressions != null)
                {
                    foreach (var expression in expressions)
                    {
                        foreach (var pattern in expression.PatternNodes)
                        {
                            clone.Matches.Clear();
                            clone.CurrentPosition = startPos;
                            clone.StartRangeMeasure();
                            var result = pattern.Parse(clone);

                            if (!result.IsSuccess)
                            {
                                clone.UndoRangeMeasure();
                                continue;
                            }

                            var exprResult = clone.Matches;
                            if (exprResult.Count > 0)
                            {
                                var cloneParseContext = clone.Clone();
                                result.Context = cloneParseContext;
                                var expr = new SkriptExpression(expression, clone.EndRangeMeasure(), cloneParseContext);
                                possibleValues.Add((clone.CurrentPosition,
                                    new List<ParseMatch>(new[] {new ExpressionParseMatch(expr, null)}), result));
                            }
                        }
                    }
                }
            }

            possibleValues.Sort((c1, c2) => -1 * c1.CurrentPosition.CompareTo(c2.CurrentPosition));

            if (possibleValues.Count <= 0) return null;

            {
                var (lastPos, matches, _) = possibleValues[0];

                ctx.ReadUntilPosition(lastPos);
                return new WrappedObject(matches);
            }
        }

        public override string AsString(WrappedObject obj)
        {
            return obj.ToString();
        }
    }
}