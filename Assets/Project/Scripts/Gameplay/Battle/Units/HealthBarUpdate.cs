namespace Project.Scripts.Gameplay.Battle.Units
{
    public enum HealthBarUpdateMode : byte
    {
        Snap,
        Damage,
        Heal
    }

    public readonly struct HealthBarUpdate
    {
        public float Fill { get; }
        public HealthBarUpdateMode Mode { get; }
        public int CurrentHP { get; }
        public int MaxHP { get; }


        public HealthBarUpdate(float fill, HealthBarUpdateMode mode, int currentHP, int maxHP)
        {
            Fill = fill;
            Mode = mode;
            CurrentHP = currentHP;
            MaxHP = maxHP;
        }
    }
}