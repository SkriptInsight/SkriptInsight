namespace SkriptInsight.Core.Types
{
    public class EnchantmentType
    {
        public EnchantmentType(SkriptEnumValue<Enchantment> enchantment, bool hasExplicitLevel, double level = 1)
        {
            Enchantment = enchantment;
            HasExplicitLevel = hasExplicitLevel;
            Level = level;
        }

        public SkriptEnumValue<Enchantment> Enchantment { get; set; }

        public double Level { get; set; }

        public bool HasExplicitLevel { get; set; }

        public bool ShouldRenderLevel => HasExplicitLevel || Level > 1;

        public override string ToString()
        {
            return $"{Enchantment}{(ShouldRenderLevel ? $" {Level}" : "")}";
        }
    }
}