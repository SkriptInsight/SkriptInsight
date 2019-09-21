using System;
using System.Collections.Generic;
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
                node.IsSectionNode ? addon.Events.Cast<AbstractSyntaxElement>() : addon.Effects))
            {
                foreach (var effect in elements)
                {
                    foreach (var effectPattern in effect.PatternNodes)
                    {
                        context.Matches.Clear();
                        context.CurrentLine = lineNumber;
                        var result = effectPattern.Parse(context);
                        if (result.IsSuccess)
                        {
                            node.MatchedSyntax = new SyntaxMatch(effect, result);
                            workDone = true;
                        }

                        if (workDone) break;
                    }

                    if (workDone) break;
                }

                if (workDone) break;
            }

/*
            var diags = new List<Diagnostic>();
            if (node.MatchedSyntax != null)
            {
                var result = node.MatchedSyntax.Result;
                
                diags.AddRange(result.Matches.OfType<ExpressionParseMatch>()
                    .SelectMany(r => r.Expression.GetValues<string>())
                    .Select(resultMatch => new Diagnostic
                    {
                        Code = "1",
                        Message = "Found a String",
                        Range = resultMatch.Range,
                        Severity = DiagnosticSeverity.Warning,
                        Source = "SkriptInsight"
                    }));
                
                diags.AddRange(result.Matches.OfType<ExpressionParseMatch>()
                    .SelectMany(r => r.Expression.GetValues<SkriptVariable>())
                    .Select(resultMatch => new Diagnostic
                    {
                        Code = "2",
                        Message = "Found a variable. REEEEEE",
                        Range = resultMatch.Range,
                        Severity = DiagnosticSeverity.Error,
                        Source = "SkriptInsight"
                    }));
            }
*/

            /*WorkspaceManager.Instance.Current.Server.Document.PublishDiagnostics(new PublishDiagnosticsParams
            {
                Uri = file.Url,
                Diagnostics = diags
            })*/;
        }
    }
}