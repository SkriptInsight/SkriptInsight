using JetBrains.Annotations;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Extensions;

namespace SkriptInsight.Core.Parser.Expressions
{
    /// <summary>
    /// Represents an annotation given to an expression whilst parsing.
    /// <para>Can be used to show warnings or errors to the user without requiring an extra code inspection.</para>
    /// </summary>
    public class MatchAnnotation
    {
        public MatchAnnotation(MatchAnnotationSeverity severity, string code, string message)
        {
            Severity = severity;
            Code = code;
            Message = message;
        }
        
        public MatchAnnotation(MatchAnnotationSeverity severity, MatchAnnotationCode code, string message) : this(severity, code.ToString(), message)
        { }
        
        public MatchAnnotation(MatchAnnotationSeverity severity, MatchAnnotationCode code) : this(severity, code.ToString(), code.GetDescription())
        { }

        public MatchAnnotationSeverity Severity { get; set; }

        public string Code { get; set; }

        public string Message { get; set; }

        public bool ShouldBeDiagnostic => Severity != MatchAnnotationSeverity.None;

        [CanBeNull]
        [ContractAnnotation("expr:null => null")]
        public Diagnostic ToDiagnostic([CanBeNull] IExpression expr)
        {
            if (expr != null && ShouldBeDiagnostic)
                return new Diagnostic
                {
                    Severity = (DiagnosticSeverity) Severity,
                    Code = Code,
                    Message = Message,
                    Range = expr.Range,
                    Source = "SkriptInsight"
                };
            
            return null;
        }
    }
}