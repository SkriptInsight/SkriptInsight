namespace SkriptInsight.Core.Types
{
    public class EntityType
    {
        public EntityType(double count, bool hasExplicitCount, EntityData entityData)
        {
            Count = count;
            HasExplicitCount = hasExplicitCount;
            EntityData = entityData;
        }

        public double Count { get; set; }

        public bool HasExplicitCount { get; set; }
        
        public EntityData EntityData { get; set; }
        
        public bool ShouldRenderCount => HasExplicitCount || Count > 1;
        
        public override string ToString()
        {
            return $"{(ShouldRenderCount ? $"{Count} " : "")}{EntityData}";
        }

    }
}