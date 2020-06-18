using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Managers;
using SkriptInsight.Core.Parser;
using SkriptInsight.Core.SyntaxInfo;

namespace SkriptInsight.Core.Files.Processes.Impl
{
    [Description("Grammatically parsing code")]
    public class ProcTryParseEffects : FileProcess
    {
        public override string ReportProgressTitle => this.GetClassDescription();
        public override string ReportProgressMessage => "";

        public override void DoWork(SkriptFile file, int lineNumber, string rawContent, FileParseContext context)
        {
            var node = file.Nodes[lineNumber];

            //Only attempt to parse already matched syntax if the user is changing the contents of the file
            if (!context.File.IsDoingNodesChange && node.MatchedSyntax != null)
                return;

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