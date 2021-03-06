using System.Collections.Generic;
using SkriptInsight.Core.Parser.Expressions;
using SkriptInsight.Core.Parser.Patterns.Impl;
using SkriptInsight.Core.SyntaxInfo;
using static SkriptInsight.Core.Managers.WorkspaceManager;

namespace SkriptInsight.Core.Parser.Types.Impl
{
    [TypeDescription("classinfo")]
    public class SkriptClassInfo : SkriptGenericType<SkriptType>
    {
        protected override SkriptType TryParse(ParseContext ctx, List<MatchAnnotation> matchAnnotationsHolder)
        {
            var litElement = new LiteralPatternElement("");
            var startPos = ctx.CurrentPosition;
            var clone = ctx.Clone();
            (int finalPos, SkriptType finalType)? regexMatch = null;

            foreach (var type in CurrentWorkspace.TypesManager.KnownTypesFromAddons)
            {
                clone.CurrentPosition = startPos;
                litElement.Value = type.FinalTypeName;
                var result = litElement.Parse(clone);

                if (!result.IsSuccess)
                {
                    //The name wasn't matched so try with regex
                    var isRegexSuccess = false;
                    if (type.LoosePatternsRegexps != null)
                    {
                        foreach (var regex in type.LoosePatternsRegexps)
                        {
                            if (isRegexSuccess) break;
                            clone.CurrentPosition = startPos;

                            var match = regex.Match(clone.PeekUntilEnd());
                            if (!match.Success) continue;

                            regexMatch = (clone.CurrentPosition + match.Length, type);
                            isRegexSuccess = true;
                        }
                    }

                    continue;
                }

                if (CurrentWorkspace.WorkspaceManager.KnownTypesManager.GetTypeByName(type.TypeName) != null)
                {
                    matchAnnotationsHolder.Add(new MatchAnnotation(MatchAnnotationSeverity.Error,
                        MatchAnnotationCode.CodeUsesInternalType));
                }

                ctx.CurrentPosition = clone.CurrentPosition;
                return type;
            }

            if (regexMatch != null)
            {
                ctx.CurrentPosition = regexMatch.Value.finalPos;
                return regexMatch.Value.finalType;
            }

            return null;
        }

        public override string AsString(SkriptType obj)
        {
            return obj.FinalTypeName;
        }
    }
}