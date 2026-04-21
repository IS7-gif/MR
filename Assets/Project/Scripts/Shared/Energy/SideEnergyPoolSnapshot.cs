namespace Project.Scripts.Shared.Energy
{
    public readonly struct SideEnergyPoolSnapshot
    {
        public float CurrentEnergy { get; }


        public SideEnergyPoolSnapshot(float currentEnergy)
        {
            CurrentEnergy = currentEnergy < 0f ? 0f : currentEnergy;
        }
    }
}
