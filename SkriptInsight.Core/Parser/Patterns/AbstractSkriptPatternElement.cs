using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Apache.NBCEL;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Parser.Patterns.Impl;

namespace SkriptInsight.Core.Parser.Patterns
{
    [DebuggerDisplay("{" + nameof(RenderPattern) + "()}")]
    public abstract class AbstractSkriptPatternElement
    {
        public int ElementIndex { get; set; } = -1;

        public AbstractSkriptPatternElement Parent { get; set; }
        
        public abstract ParseResult Parse(ParseContext contextToUse);

        public abstract string RenderPattern();

        public override string ToString()
        {
            return RenderPattern();
        }


        public bool NarrowContextIfPossible(ref ParseContext contextToUse, out ParseResult parseResult)
        {
            //Try to narrow the context by matching the next pattern element
            //Return value tells us if we should fast-fail.
            var result = TryToNarrowContext(ref contextToUse, out var narrowedResult);
            if (!result)
            {
                parseResult = ParseResult.Failure(contextToUse);
                return true;
            }

            //If we got a match from before 
            if (narrowedResult != null)
            {
                parseResult = narrowedResult;
                return true;
            }

            parseResult = null;
            return false;
        }

        protected bool TryToNarrowContext(ref ParseContext ctx, out ParseResult result)
        {
            result = null;
            List<AbstractSkriptPatternElement> parentElements;
            AbstractSkriptPatternElement parent = this;
            {
                while (parent?.Parent != null)
                {
                    parent = parent.Parent;
                }


                parentElements = PatternHelper.Flatten(parent).ToList();
            }
        
            //if (parent != null && parent.ToString().Contains("%object"))
            //    Debugger.Break();

            var indexOfThis = ElementIndex;
            if (indexOfThis == -1) return true;

            var fromIndex = indexOfThis + 1;
            if (fromIndex > parentElements.Count) return true;

            if (parent.ToString().Contains("exp"))
            {
                //Debugger.Break();
            }
            
            var possibleInputs =
                PatternHelper.GetPossibleInputs(parentElements.SubList(fromIndex, parentElements.Count));

            if (possibleInputs == null) return true;
            var anyMatch = false;
            foreach (var element in possibleInputs)
            {
                switch (element)
                {
                    case LiteralPatternElement literal:
                        var index = ctx.Text.SafeSubstring(ctx.CurrentPosition).IndexOf(literal.Value, StringComparison.InvariantCultureIgnoreCase);
                        
                        if (index > -1)
                        {
                            var clone = ctx;
                            clone.MaxLengthOverride = ctx.CurrentPosition + index;
                            ctx = clone;
                            anyMatch = true;
                        }
                        
                        break;
                }
            }
            //TODO: Return true if anyMatch is true
            return true;
        }
        
        protected static void RestoreFromNarrowedContext(ParseContext ctx, ParseContext narrowedContext)
        {
            narrowedContext.MaxLengthOverride = -1;

            if (narrowedContext.CurrentPosition > ctx.CurrentPosition)
                ctx.ReadUntilPosition(narrowedContext.CurrentPosition);

            //This can only be true if the context has been narrowed before
            if (ctx != narrowedContext)
            {
                ctx.Matches.AddRange(narrowedContext.Matches.ToList().Select(c =>
                {
                    c.Context = ctx;
                    return c;
                }));
            }
        }

    }
}