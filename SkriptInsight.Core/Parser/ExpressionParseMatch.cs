using System;
using System.Collections.Generic;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Parser.Expressions;

namespace SkriptInsight.Core.Parser
{
    /// <summary>
    /// Parse match that matched an expression 
    /// </summary>
    public class ExpressionParseMatch : ParseMatch
    {
        public ExpressionParseMatch(IExpression expression)
        {
            int ResolveFor(Position pos, Position otherPos, List<string> list)
            {
                if (pos.Line.Equals(otherPos.Line))
                    return (int) pos.Character;
                return pos.ResolveFor(list);
            }

            Expression = expression;
            Range = expression.Range;
            Context = expression.Context;
            
            var lines = expression.Context.Text.SplitOnNewLines();
            var startPos = ResolveFor(Range.Start, Range.End, lines);
            var endPos = ResolveFor(Range.End, Range.Start, lines);
            RawContent = expression.Context.Text.Substring(startPos, endPos - startPos);
        }

        public IExpression Expression { get; set; }
    }
}