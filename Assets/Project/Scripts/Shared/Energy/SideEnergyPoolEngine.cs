namespace Project.Scripts.Shared.Energy
{
    public class SideEnergyPoolEngine
    {
        public SideEnergyPoolSnapshot Snapshot => new(_currentEnergy);


        private float _currentEnergy;


        public float AddEnergy(float amount)
        {
            if (amount <= 0f)
                return 0f;

            _currentEnergy += amount;
            
            return amount;
        }

        public bool CanSpend(int amount)
        {
            return amount > 0 && _currentEnergy >= amount;
        }

        public bool TrySpend(int amount)
        {
            if (false == CanSpend(amount))
                return false;

            _currentEnergy -= amount;
            
            return true;
        }

        public void Reset()
        {
            _currentEnergy = 0f;
        }
    }
}