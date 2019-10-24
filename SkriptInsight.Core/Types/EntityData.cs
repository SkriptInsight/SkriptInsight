using System;
using SkriptInsight.Core.Parser;
using SkriptInsight.Core.SyntaxInfo;

namespace SkriptInsight.Core.Types
{
    public class EntityData
    {
        public Noun UsedNoun { get; set; }
        
        public EntityTypeUsedValues UsedValues { get; set; }

        [Flags]
        public enum EntityTypeUsedValues
        {
            Gender = 1,
            Singular = 1 << 1,
            Plural = 1 << 2
        }

        public EntityData(Noun usedNoun, EntityTypeUsedValues usedValues)
        {
            UsedNoun = usedNoun;
            UsedValues = usedValues;
        }


        public override string ToString()
        {
            return
                // ReSharper disable once UseStringInterpolation
                string.Format("{0}{1}{2}",
                    UsedValues.HasFlagFast(EntityTypeUsedValues.Gender) ? UsedNoun.Gender + ' ' : "",
                    UsedValues.HasFlagFast(EntityTypeUsedValues.Singular) ? UsedNoun.Singular : "",
                    UsedValues.HasFlagFast(EntityTypeUsedValues.Plural) ? UsedNoun.Plural : "");
        }
    }

    public static class UsedValuesExtensions
    {
        public static bool HasFlagFast(this EntityData.EntityTypeUsedValues value, EntityData.EntityTypeUsedValues flag)
        {
            return (value & flag) != 0;
        }
    }
}