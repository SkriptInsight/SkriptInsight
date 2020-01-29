using System.Collections.Generic;
using JetBrains.Annotations;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Parser.Types;

namespace SkriptInsight.Core.Parser.Expressions
{
    /// <summary>
    /// Represents a Skript expression that can be read from and turned to text
    /// </summary>
    public interface IExpression
    {
        object Value { get; set; }

        ISkriptType Type { get; set; }

        Range Range { get; set; }

        ParseContext Context { get; set; }

        string AsString();

        
        /// <summary>
        /// The match annotations on this expression.
        /// </summary>
        [NotNull]
        public List<MatchAnnotation> MatchAnnotations { get; set; }
    }
}