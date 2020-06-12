using SkriptInsight.Core.Types.Attributes;

namespace SkriptInsight.Core.Types
{
    public enum Enchantment
    {
        [PatternAlias("Protection")] Protection,
        [PatternAlias("Fire Protection")] FireProtection,
        [PatternAlias("Feather Falling")] FeatherFalling,
        [PatternAlias("Blast Protection")] BlastProtection,

        [PatternAlias("Projectile Protection")]
        ProjectileProtection,
        [PatternAlias("Respiration")] Respiration,
        [PatternAlias("Aqua Affinity")] AquaAffinity,
        [PatternAlias("Sharpness")] Sharpness,
        [PatternAlias("Smite")] Smite,
        [PatternAlias("Bane of Arthropods")] BaneOfArthropods,
        [PatternAlias("Knockback")] Knockback,
        [PatternAlias("Fire Aspect")] FireAspect,
        [PatternAlias("Looting")] Looting,
        [PatternAlias("Efficiency")] Efficiency,
        [PatternAlias("Silk Touch")] SilkTouch,
        [PatternAlias("Unbreaking")] Unbreaking,
        [PatternAlias("Fortune")] Fortune,
        [PatternAlias("Power")] Power,
        [PatternAlias("Punch")] Punch,
        [PatternAlias("Flame")] Flame,
        [PatternAlias("Infinity")] Infinity,
        [PatternAlias("Thorns")] Thorns,
        [PatternAlias("Luck of the Sea")] Luck,
        [PatternAlias("Lure")] Lure,
        [PatternAlias("Depth Strider")] DepthStrider,
        [PatternAlias("Mending")] Mending,
        [PatternAlias("Frost Walker")] FrostWalker,
        [PatternAlias("Curse of Vanishing")] VanishingCurse,
        [PatternAlias("Curse of Binding")] BindingCurse,
        [PatternAlias("Sweeping Edge")] Sweeping,

        [PatternAlias("Channeling", "Channelling")]
        Channeling,
        [PatternAlias("Riptide")] Riptide,
        [PatternAlias("Impaling")] Impaling,
        [PatternAlias("Loyalty")] Loyalty,
        [PatternAlias("Luck of The Sea")] LuckOfTheSea,

        [PatternAlias("Multishot", "Multi-Shot")]
        Multishot,
        [PatternAlias("Piercing")] Piercing,
        [PatternAlias("Quick Charge")] QuickCharge,
    }
}