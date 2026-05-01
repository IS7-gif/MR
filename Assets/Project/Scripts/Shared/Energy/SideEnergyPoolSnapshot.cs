namespace Project.Scripts.Shared.Energy
{
    public readonly struct SideEnergyPoolSnapshot
    {
        public float CurrentEnergy { get; }
        public float EnergyCap { get; }


        public SideEnergyPoolSnapshot(float currentEnergy, float energyCap)
        {
            EnergyCap = energyCap < 0f ? 0f : energyCap;
            var safeCurrent = currentEnergy < 0f ? 0f : currentEnergy;
            CurrentEnergy = EnergyCap > 0f && safeCurrent > EnergyCap
                ? EnergyCap
                : safeCurrent;
        }
    }
}