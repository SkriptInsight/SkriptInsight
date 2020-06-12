using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Extensions;

namespace SkriptInsight.Core.Inspections.Problems
{
    public class ProblemsHolder
    {
        private ConcurrentDictionary<Range, ProblemDefinition> ProblemsList { get; } =
            new ConcurrentDictionary<Range, ProblemDefinition>();

        public IReadOnlyCollection<ProblemDefinition> Problems =>
            ProblemsList.Values as IReadOnlyCollection<ProblemDefinition>;

        public void Add(ProblemDefinition def)
        {
            ProblemsList[def.Range] = def;
        }

        public void Clear(int startLine, int endLine)
        {
            var toRemove = ProblemsList.Keys
                .Where(key => key.Start.Line >= startLine && key.End.Line <= endLine).ToList();
            toRemove.ForEach(r => ProblemsList.TryRemove(r, out _));
        }

        public void ShiftLineNumber(in int amount)
        {
            var values = Problems;

            foreach (var def in values)
            {
                def.Range.ShiftLineNumber(amount);
            }
        }
    }
}