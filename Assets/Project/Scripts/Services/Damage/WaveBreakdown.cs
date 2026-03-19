namespace Project.Scripts.Services.Damage
{
    public readonly struct WaveBreakdown
    {
        public readonly int CascadeLevel;
        public readonly int MatchCount;
        public readonly int RawDamage;
        public readonly float CascadeMultiplier;
        public readonly bool HasMultiMatchBonus;
        public readonly int Total;


        public WaveBreakdown(int cascadeLevel, int matchCount, int rawDamage,
            float cascadeMultiplier, bool hasMultiMatchBonus, int total)
        {
            CascadeLevel = cascadeLevel;
            MatchCount = matchCount;
            RawDamage = rawDamage;
            CascadeMultiplier = cascadeMultiplier;
            HasMultiMatchBonus = hasMultiMatchBonus;
            Total = total;
        }
    }
}
