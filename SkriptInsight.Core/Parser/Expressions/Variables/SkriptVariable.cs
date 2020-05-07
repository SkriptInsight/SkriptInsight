using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Parser.Expressions.Variables.Content;

namespace SkriptInsight.Core.Parser.Expressions.Variables
{
    public class SkriptVariable
    {
        public List<VariableContent> Contents { get; } = new List<VariableContent>();

        public override string ToString()
        {
            return $"{{{string.Join("", Contents.Select(c => c.RenderContent()))}}}";
        }

        [CanBeNull]
        public static SkriptVariable TryParse(ParseContext ctx)
        {
            if (ctx.ReadNext(1) == "{")
            {
                var variable = new SkriptVariable();
                var nextBracket = ctx.FindNextBracket('{', '}', matchExclusions: new[] {('{', '}')});
                if (nextBracket < 0) return null;
                
                var content = ctx.ReadUntilPosition(nextBracket);
                var builder = new StringBuilder();
                var onVariableSplit = false;

                void AddLiteralContentToVariable()
                {
                    if (string.IsNullOrEmpty(builder.ToString())) return;
                    var variableContent = new StringLiteralVariableContent(builder.ToString());
                    variable.Contents.Add(onVariableSplit ? (VariableContent) new SplitLiteralContent(variableContent) : variableContent);
                    builder.Clear();
                }

                var context = ParseContext.FromCode(content);
                var ignoreUntil = -1;
                var cs = context.ToList().WithContext(false, true);
                for (var currPos = 0; currPos < cs.Count; currPos++)
                {
                    context.CurrentPosition = currPos + 1;
                    var c = cs[currPos];
                    if (ignoreUntil > 0 && context.CurrentPosition <= ignoreUntil) continue;
                    ignoreUntil = -1;

                    if (c.Current == ':' && c.Next == ':')
                    {
                        AddLiteralContentToVariable();
                        onVariableSplit = true;
                        ignoreUntil = context.CurrentPosition + 1;
                    }
                    else if (c.Current == '%')
                    {
                        AddLiteralContentToVariable();
                        var exprEnd = context.FindNextBracket('%', true);
                        if (exprEnd > 0)
                        {
                            var expression = context.ReadUntilPosition(exprEnd);
                            ignoreUntil = exprEnd + 1;
                            variable.Contents.Add(new ExpressionVariableContent(expression));
                        }
                    }
                    else
                    {
                        builder.Append(c.Current);
                    }
                }

                AddLiteralContentToVariable();
                return variable;
            }

            return null;
        }
    }
}