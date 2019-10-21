using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Parser.Patterns;
using SkriptInsight.Core.Parser.Patterns.Impl;

namespace SkriptInsight.Core.Parser
{
    public static class SkriptNounParser
    {
        private const char BrokenPipe = '¦';
        
        public static (string Gender, string Singular, string Plural) ParseNoun(string noun)
        {
            var gender = "";
            var genderIndex = noun.IndexOf("@", StringComparison.Ordinal);
            if (genderIndex - 1 > -1)
            {
                gender = noun.Substring(genderIndex + 1);
                noun = noun.Substring(0, genderIndex - 1);
            }

            var brokenPipesCount = noun.Count(c => c.Equals(BrokenPipe));

            string singular = "", plural = "";
            switch (brokenPipesCount)
            {
                // No broken pipe.
                // This means that it is just singular.
                // Example: spectral arrow
                case 0:
                    singular = noun;
                    break;
                
                // Just one broken pipe.
                // This means that it just splits between the singular and plural.
                // Example: tree¦s
                case 1:
                    var indexOfFirst = noun.IndexOf(BrokenPipe);
                    singular = noun.Substring(0, indexOfFirst);
                    plural = singular + noun.Substring(indexOfFirst + 1);
                    break;
                
                // Two broken pipes.
                // This means that it splits between the singular and plural and has a common prefix on both.
                // Example: zombie pig
                case 2:
                    var indexFirstPipe = noun.IndexOf(BrokenPipe);
                    var indexSecondPipe = noun.IndexOf(BrokenPipe, indexFirstPipe + 1);
                    var commonPrefix = noun.Substring(0, indexFirstPipe);
                    
                    singular = commonPrefix + noun.Substring(indexFirstPipe + 1, indexSecondPipe - (indexFirstPipe + 1));
                    plural = commonPrefix + noun.Substring(indexSecondPipe + 1);
                    break;
            }
            

            return (gender, singular, plural);
        }

        public static SkriptPattern ConvertNounToPattern(string noun)
        {
            var pattern = new SkriptPattern();

            var (gender, singular, plural) = ParseNoun(noun);

            var singularPattern = new SkriptPattern();
            
            //Add singular gender into the singular literal if it exists
            if (!gender.IsEmpty())
                singularPattern.Children.Add(new OptionalPatternElement
                {
                    Element = new LiteralPatternElement(gender + ' ')
                });
            singularPattern.Children.Add(new LiteralPatternElement(singular));
            
            //Create final element
            AbstractSkriptPatternElement toAdd;
            if (!plural.IsEmpty())
            {
                toAdd = new ChoicePatternElement
                {
                    Elements = new List<ChoicePatternElement.ChoiceGroupElement>
                    {
                        new ChoicePatternElement.ChoiceGroupElement(singularPattern),
                        new ChoicePatternElement.ChoiceGroupElement(new LiteralPatternElement(plural))
                    }
                };
            }
            else
                toAdd = singularPattern;

            pattern.Children.Add(toAdd);
            
            return pattern;
        }
    }
}