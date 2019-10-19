using System.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Parser.Expressions;
using SkriptInsight.Core.Parser.Patterns;
using SkriptInsight.Core.Parser.Patterns.Impl;

namespace SkriptInsight.Core.Parser.Types.Impl.Generic
{
    public class GenericParenthesesType : ISkriptType
    {
        public GenericParenthesesType(string type)
        {
            InnerPattern = CreateInnerPattern(type + (!type.EndsWith("s") ? "s" : ""));
        }

        private static SkriptPattern CreateInnerPattern(string typeName)
        {
            return new SkriptPattern
            {
                Children =
                {
                    new TypePatternElement
                    {
                        Constraint = SyntaxValueAcceptanceConstraint.None,
                        Type = typeName
                    }
                }
            };
        }

        public SkriptPattern InnerPattern { get; set; }

        public IExpression TryParseValue(ParseContext ctx)
        {
            ctx.StartRangeMeasure();
            if (ctx.PeekNext(1) == "(")
            {
                ctx.ReadNext(1); //Read the first bracket
                var nextBracket = ctx.FindNextBracket('(', ')');

                if (nextBracket != -1)
                {
                    var ctxClone = ctx.Clone(false);

                    var result = InnerPattern.Parse(ctxClone);
                    var value = result.Matches.OfType<ExpressionParseMatch>().FirstOrDefault()?.Expression;
                    var oldPos = ctxClone.CurrentPosition;
                    if (result.IsSuccess) ctx.ReadUntilPosition(oldPos);

                    if (value != null && ctx.ReadNext(1) == ")")
                    {
                        return new ParenthesesExpression(value, this)
                        {
                            Range = ctx.EndRangeMeasure()
                        };
                    }

                    //Rollback in case of it not working
                    ctx.CurrentPosition = oldPos;
                }
            }
            ctx.UndoRangeMeasure();
            return null;
        }

        public string AsString(object obj)
        {
            return $"({(obj as ParenthesesExpression)?.InnerExpression?.AsString() ?? "<none>"})";
        }

        public object Value { get; set; }
        
        public ISkriptType Type { get; set; }
        
        public Range Range { get; set; }
        
        public ParseContext Context { get; set; }
        
    }
}