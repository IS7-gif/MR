namespace Project.Scripts.Shared.Avatar
{
    public readonly struct AvatarEnergyState
    {
        public int CurrentEnergy { get; }
        public int MaxEnergy { get; }
        public bool IsReady => MaxEnergy > 0 && CurrentEnergy >= MaxEnergy;
        public bool IsEmpty => CurrentEnergy <= 0;
        public float FillFraction => MaxEnergy > 0 ? (float)CurrentEnergy / MaxEnergy : 0f;


        public AvatarEnergyState(int currentEnergy, int maxEnergy)
        {
            CurrentEnergy = currentEnergy;
            MaxEnergy = maxEnergy;
        }
    }
}