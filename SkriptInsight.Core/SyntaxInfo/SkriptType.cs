using System;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Humanizer;
using SkriptInsight.Core.Parser;
using SkriptInsight.Core.Parser.Expressions;
using SkriptInsight.Core.Parser.Types.Impl.Internal;

namespace SkriptInsight.Core.SyntaxInfo
{
    public class SkriptType
    {
        public static readonly SkriptType Void = new SkriptType
        {
            AddonName = "Skript",
            Id = 0,
            Since = "Skript 1.0",
            ClassName = "Void",
            TypeName = "void",
            Description = new[]
                {"Default void type. Used by SkriptInsight to denote the lack of a return type on functions"}
        };

        public static readonly Expression<SkriptType> VoidExpr = new Expression<SkriptType>(Void)
        {
            Type = new SkriptVoid()
        };

        private string _typeName;
        private bool _isPlural;

        public int Id { get; set; }

        public string[] Description { get; set; }

        public string[] Usage { get; set; }

        public string[] Examples { get; set; }

        public string Since { get; set; }

        public string TypeName
        {
            get => _typeName;
            set
            {
                _typeName = value;
                UpdateFinalTypeName();
            }
        }

        private string CalculateFinalName()
        {
            return !IsPlural ? TypeName.Singularize(false) : TypeName.Pluralize(false);
        }

        public string AddonName { get; set; }

        public string ClassName { get; set; }

        public string[] PossibleValues { get; set; }

        public Noun[] PossibleValuesAsNouns { get; set; }

        public string[] Patterns { get; set; }

        public Regex[] PatternsRegexes { get; set; }

        public void LoadPatterns()
        {
            PossibleValuesAsNouns = PossibleValues?.Select(SkriptNounParser.ParseNoun).ToArray();
            PatternsRegexes = Patterns?
                .Select(c => new Regex('^' + c + '$', RegexOptions.Compiled | RegexOptions.IgnoreCase))
                .ToArray();
        }

        [JsonIgnore]
        public bool IsPlural
        {
            get => _isPlural;
            set
            {
                _isPlural = value; 
                UpdateFinalTypeName();
            }
        }

        private string UpdateFinalTypeName()
        {
            return FinalTypeName = CalculateFinalName();
        }

        public SkriptType Clone()
        {
            return new SkriptType
            {
                Id = Id,
                AddonName = AddonName,
                TypeName = TypeName,
                Description = Description,
                Examples = Examples,
                Patterns = Patterns,
                Since = Since,
                Usage = Usage,
                ClassName = ClassName,
                PatternsRegexes = PatternsRegexes,
                PossibleValues = PossibleValues,
                PossibleValuesAsNouns = PossibleValuesAsNouns,
                IsPlural = IsPlural
            };
        }
        
        public string FinalTypeName { get; set; }
        
        public override string ToString()
        {
            return FinalTypeName;
        }
    }
}