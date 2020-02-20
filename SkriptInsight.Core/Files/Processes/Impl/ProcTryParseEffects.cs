using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Managers;
using SkriptInsight.Core.Parser;
using SkriptInsight.Core.Parser.Expressions;
using SkriptInsight.Core.Parser.Expressions.Variables;
using SkriptInsight.Core.Parser.Types.Impl;
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
                node.IsSectionNode ? addon.Events : addon.Expressions.Cast<AbstractSyntaxElement>().Concat(addon.Effects)))
            {
                foreach (var effect in elements)
                {
                    foreach (var effectPattern in effect.PatternNodes)
                    {
                        // if ((effect as SyntaxSkriptExpression)?.ClassName == "com.btk5h.skriptmirror.skript.reflect.ExprJavaCall")
                            // Debugger.Break();
                            
                        context.Matches = new List<ParseMatch>();
                        context.CurrentLine = lineNumber;
                        var result = effectPattern.Parse(context);
                        if (result.IsSuccess)
                        {
                            result.Context = context.Clone();
                            result.Matches = result.Context.Matches;
                            node.MatchedSyntax = new SyntaxMatch(effect, result);
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