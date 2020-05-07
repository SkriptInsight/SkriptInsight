using System;
using System.Linq;
using System.Runtime.CompilerServices;
using SkriptInsight.Core.Files;
using SkriptInsight.Core.Managers;
using SkriptInsight.Core.Parser.Expressions;
using SkriptInsight.Core.Parser.Types;
using SkriptInsight.Core.Parser.Types.Impl;
using SkriptInsight.Core.Parser.Types.Impl.Generic;
using SkriptInsight.Core.SyntaxInfo;

namespace SkriptInsight.Core.Parser.Patterns.Impl
{
    [GroupPatternElementInfo('%')]
    public class TypePatternElement : AbstractGroupPatternElement
    {
        public TypePatternElement(string contents) : base(contents)
        {
            var list = contents.TakeWhile(c => ParseConstraint(c) != SyntaxValueAcceptanceConstraint.None)
                .Select(ParseConstraint).ToList();

            Type = contents.Substring(list.Count);
            if (!list.Any()) list.Add(SyntaxValueAcceptanceConstraint.None);
            Constraint = list.Aggregate((a, b) => a | b);
        }

        public TypePatternElement()
        {
        }

        public SyntaxValueAcceptanceConstraint Constraint { get; set; }

        public string Type { get; set; }

        public bool SkipParenthesis { get; set; }

        /// <summary>
        ///     Can this type, when matching list values, match 'and' or 'or'
        /// </summary>
        public bool CanMatchListConjunctions { get; set; } = true;

        public string ConstraintAsString => string.Join("", Enum.GetValues(typeof(SyntaxValueAcceptanceConstraint))
            .Cast<SyntaxValueAcceptanceConstraint>()
            .Where(c => Constraint.HasFlagFast(c))
            .Select(RenderConstraint));

        public override ParseResult Parse(ParseContext ctx)
        {
            var skriptTypesManager = WorkspaceManager.CurrentWorkspace.TypesManager;

            //Split the types (if any)
            foreach (var typeRaw in Type.Split("/"))
            {
                var skriptType = skriptTypesManager.GetType(typeRaw);
                //Try to get a known type literal provider for that type
                var type = WorkspaceManager.Instance.KnownTypesManager.GetTypeByName(typeRaw);
                ISkriptType skriptTypeDescriptor = null;

                if (!RuntimeHelpers.TryEnsureSufficientExecutionStack())
                    return ParseResult.Failure(ctx);

                //Check if this type requires more than one variable
                var isMultipleValues = typeRaw.EndsWith("s");
                if (isMultipleValues)
                {
                    //It does so we first have to get the singular type representation for this type
                    //Either get from the name we're given or just subtract the last letter ('s') from it
                    type = WorkspaceManager.Instance.KnownTypesManager.GetTypeByName(typeRaw) ??
                           WorkspaceManager.Instance.KnownTypesManager.GetTypeByName(
                               typeRaw.Substring(0, typeRaw.Length - 1)
                           );

                    //If we have a type, replace the type descriptor for the generic multi type matcher
                    if (type != null) // Hand over to GenericMultiValueType
                        skriptTypeDescriptor = new GenericMultiValueType(type, Constraint, CanMatchListConjunctions);
                }
                else
                {
                    //We got a single type so, use it.
                    skriptTypeDescriptor = type?.CreateNewInstance();
                }

                IExpression result = null;
                var oldPos = ctx.CurrentPosition;

                if (!ctx.ShouldJustCheckExpressionsThatMatchType)
                {
                    result = skriptTypeDescriptor?.TryParseValue(ctx);
                }

                //This type doesn't have a flag to just match literals So, let's try first matching variables.
                if (result == null && !Constraint.HasFlagFast(SyntaxValueAcceptanceConstraint.LiteralsOnly))
                {
                    //Try parsing a variable
                    var reference = new SkriptVariableReferenceType();
                    var ctxClone = ctx.Clone();
                    try
                    {
                        result = reference.TryParseValue(ctxClone);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    if (result != null) ctx.ReadUntilPosition(ctxClone.CurrentPosition);
                }


                //We have not matched a result. Let's try matching with parentheses
                if (result == null && !SkipParenthesis)
                {
                    // Type descriptor wasn't able to parse the literal. Push back and try with parentheses.
                    ctx.CurrentPosition = oldPos;

                    //Try parsing with parentheses first
                    var parenthesesType = new GenericParenthesesType(typeRaw);
                    result = parenthesesType.TryParseValue(ctx);
                }

                //If we didn't match any literal for this type, try to match an expression for this type
                if (skriptType != null && result == null &&
                    !Constraint.HasFlagFast(SyntaxValueAcceptanceConstraint.LiteralsOnly))
                {
                    var clone = ctx.Clone(false);
                    clone.ShouldJustCheckExpressionsThatMatchType = true;
                    var currentPos = clone.CurrentPosition;

                    result = TryMatchExpressionOnFile(ctx, skriptTypesManager, clone, currentPos, skriptType);

                    //Temporarily disable
                    if (false && typeRaw.ToLower() != "object" && typeRaw.ToLower() != "objects")
                    {
                        //Try matching a Skript expression
                        // Time to check all expressions to make sure the user isn't just trying to mix types for whatever reason...
                        var exprFitType = ctx.ShouldJustCheckExpressionsThatMatchType
                            ? skriptTypesManager.GetExpressionsThatCanFitType(skriptType)
                            : skriptTypesManager.KnownExpressionsFromAddons;

                        if (exprFitType != null)
                            foreach (var expression in exprFitType)
                            {
                                clone.CurrentPosition = currentPos;

                                if (clone.HasVisitedExpression(skriptType, expression)) continue;

                                clone.StartRangeMeasure("Expression");
                                clone.Matches.Clear();

                                clone.VisitExpression(skriptType, expression);

                                for (var index = 0; index < expression.PatternNodes.Length; index++)
                                {
                                    var pattern = expression.PatternNodes[index];

                                    var resultValue = pattern.Parse(clone);
                                    if (resultValue.IsSuccess && clone.CurrentPosition >= ctx.CurrentPosition)
                                    {
                                        var range = clone.EndRangeMeasure("Expression");
                                        ctx.ReadUntilPosition(clone.CurrentPosition);

                                        result = new SkriptExpression(expression, range, ctx);
                                    }
                                }

                                clone.UndoRangeMeasure();
                            }
                    }
                }

                //If we have matched something, let's add it to the matches.
                if (result != null)
                {
                    if (result.Type == null) result.Type = skriptTypeDescriptor;
                    result.Context = ctx;
                    var match = new ExpressionParseMatch(result, this);

                    ctx.Matches.Add(match);
                }

                return result != null ? ParseResult.Success(ctx) : ParseResult.Failure(ctx);
            }

            return ParseResult.Failure(ctx);
        }

        private static IExpression TryMatchExpressionOnFile(ParseContext ctx, SkriptTypesManager skriptTypesManager,
            ParseContext clone, int currentPos, SkriptType skriptType)
        {
            IExpression result = null;

            //If we're dealing with a file, try matching event values too
            if (ctx is FileParseContext fileParseContext)
            {
                var currentNode = fileParseContext.File.Nodes[fileParseContext.CurrentLine];

                if (currentNode?.RootParentSyntax?.Element is SkriptEvent rootEvent)
                {
                    var typesAndExpressions = skriptTypesManager.GetEventExpressionsForEvent(rootEvent);

                    var exprs = typesAndExpressions;

                    if (exprs != null)
                    {
                        foreach (var (_, expressions) in exprs)
                        {
                            if (expressions != null)
                            {
                                foreach (var expression in expressions)
                                {
                                    clone.CurrentPosition = currentPos;
                                    clone.StartRangeMeasure("Event-Value Expression");
                                    clone.Matches.Clear();

                                    clone.VisitExpression(skriptType, expression);

                                    for (var index = 0; index < expression.PatternNodes.Length; index++)
                                    {
                                        var pattern = expression.PatternNodes[index];
                                        var resultValue = pattern.Parse(clone);
                                        if (resultValue.IsSuccess)
                                        {
                                            var range = clone.EndRangeMeasure("Event-Value Expression");
                                            ctx.ReadUntilPosition(clone.CurrentPosition);

                                            result = new SkriptExpression(expression, range, ctx);
                                        }
                                    }

                                    clone.UndoRangeMeasure();
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        public override string RenderPattern()
        {
            return $"%{ConstraintAsString}{Type}%";
        }

        #region Constraint

        private static string RenderConstraint(SyntaxValueAcceptanceConstraint c)
        {
            return c switch
            {
                SyntaxValueAcceptanceConstraint.None => "",
                SyntaxValueAcceptanceConstraint.LiteralsOnly => "*",
                SyntaxValueAcceptanceConstraint.NoLiterals => "~",
                SyntaxValueAcceptanceConstraint.VariablesOnly => "^",
                SyntaxValueAcceptanceConstraint.NullWhenOmitted => "-",
                SyntaxValueAcceptanceConstraint.AllowConditionalExpressions => "=",
                _ => throw new ArgumentOutOfRangeException(nameof(c))
            };
        }

        private static SyntaxValueAcceptanceConstraint ParseConstraint(char c)
        {
            return c switch
            {
                '*' => SyntaxValueAcceptanceConstraint.LiteralsOnly,
                '~' => SyntaxValueAcceptanceConstraint.NoLiterals,
                '^' => SyntaxValueAcceptanceConstraint.VariablesOnly,
                '-' => SyntaxValueAcceptanceConstraint.NullWhenOmitted,
                '=' => SyntaxValueAcceptanceConstraint.AllowConditionalExpressions,
                _ => SyntaxValueAcceptanceConstraint.None
            };
        }

        #endregion
    }
}