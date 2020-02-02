using System;
using JetBrains.Annotations;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace SkriptInsight.Core.Inspections.Problems
{
    [UsedImplicitly]
    public class ProblemDefinition
    {
        public ProblemDefinition(DiagnosticSeverity severity, string id, string message, Range range)
        {
            Severity = severity;
            Id = id;
            Message = message;
            Range = range;
        }

        public DiagnosticSeverity Severity { get; set; }

        public string Id { get; set; }

        public string Message { get; set; }

        public Range Range { get; set; }


        public Diagnostic ToDiagnostic(params DiagnosticRelatedInformation[] info)
        {
            return ToDiagnostic(() => info);
        }

        public Diagnostic ToDiagnostic(Func<Container<DiagnosticRelatedInformation>> extraInfo = null)
        {
            var diagnostic = new Diagnostic
            {
                Code = Id,
                Message = Message,
                Range = Range,
                Severity = Severity,
                Source = "SkriptInsight"
            };

            if (extraInfo != null) diagnostic.RelatedInformation = extraInfo();

            return diagnostic;
        }
    }
}