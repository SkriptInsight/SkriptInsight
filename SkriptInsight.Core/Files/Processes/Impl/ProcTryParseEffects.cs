using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SkriptInsight.Core.Managers;
using SkriptInsight.Core.Parser;
using SkriptInsight.Core.SyntaxInfo;

namespace SkriptInsight.Core.Files.Processes.Impl
{
    public class ProcTryParseEffects : FileProcess
    {
        public override void DoWork(SkriptFile file, int lineNumber, string rawContent, FileParseContext context)
        {
            var node = file.Nodes[lineNumber];
            var workDone = false;
            foreach (var elements in WorkspaceManager.Instance.Current.AddonDocumentations.Select(addon =>
                node.IsSectionNode
                    ? addon.Events
                    : addon.Expressions.Cast<AbstractSyntaxElement>().Concat(addon.Effects)))
            {
                foreach (var effect in elements)
                {
                    for (var index = 0; index < effect.PatternNodes.Length; index++)
                    {
                        var effectPattern = effect.PatternNodes[index];

                        if (effect is SyntaxSkriptExpression expression2)
                            Debug.WriteLine(expression2.ClassName);
                        /* 
                         if (effect is SyntaxSkriptExpression expression && expression.ClassName.Contains("JavaCall"))
                             Debugger.Break();
                         */
                        context.Matches = new List<ParseMatch>();
                        context.CurrentLine = lineNumber;
                        var result = effectPattern.Parse(context);
                        if (result.IsSuccess)
                        {
                            result.Context = context.Clone();
                            result.Matches = result.Context.Matches;
                            node.MatchedSyntax = new SyntaxMatch(effect, result) {PatternIndex = index};
                            workDone = true;
                        }

                        if (workDone) break;
                    }

                    if (workDone) break;
                }

                if (workDone) break;
            }
        }
    }
}