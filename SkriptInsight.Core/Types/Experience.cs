using System;

namespace SkriptInsight.Core.Types
{
    public class Experience
    {
        public Experience(long amount, ExperienceType type)
        {
            Amount = amount;
            Type = type;
        }

        public Experience()
        {
        }

        public double Amount { get; set; }

        public ExperienceType Type { get; set; }

        public override string ToString()
        {
            return $"{Amount} {GetTypeAsString(Amount)}";
        }

        private string GetTypeAsString(in double amount)
        {
            switch (Type)
            {
                case ExperienceType.Xp:
                    return "xp";
                case ExperienceType.Exp:
                    return "exp";
                case ExperienceType.Points:
                    return "experience";
                case ExperienceType.ExperiencePoints:
                    return $"experience point{(Math.Abs(amount - 1) > double.Epsilon ? "s" : string.Empty)}";
                default:
                    throw new Exception("Type is unknown");
            }
        }
    }

    public enum ExperienceType
    {
        Xp = 1,
        Exp = 5, // 1 ^ 4
        Points = 2,
        ExperiencePoints = 7, // 2 ^ 5
    }

    public static class ExperienceTypeExtensions
    {
        public static bool IsXp(this ExperienceType value)
        {
            return value == ExperienceType.Exp || value == ExperienceType.Xp;
        }

        public static bool IsPoints(this ExperienceType value)
        {
            return value == ExperienceType.Points || value == ExperienceType.ExperiencePoints;
        }
    }
}