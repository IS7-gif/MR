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


        public HealthBarUpdate(float fill, HealthBarUpdateMode mode)
        {
            Fill = fill;
            Mode = mode;
        }
    }
}