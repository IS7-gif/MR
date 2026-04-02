using UnityEngine;

namespace Project.Scripts.Shared.Avatar
{
    public sealed class AvatarEnergyEngine
    {
        public AvatarEnergyState Snapshot => new AvatarEnergyState(_currentEnergy, _maxEnergy);


        private int _maxEnergy;
        private int _currentEnergy;


        public void Initialize(int maxEnergy)
        {
            if (maxEnergy <= 0)
            {
                Debug.LogError($"[AvatarEnergyEngine] maxEnergy must be > 0, got {maxEnergy}. Clamping to 1.");
                maxEnergy = 1;
            }

            _maxEnergy = maxEnergy;
            _currentEnergy = 0;
        }

        public int TryAddEnergy(int amount)
        {
            if (amount <= 0 || _currentEnergy >= _maxEnergy)
                return 0;

            var before = _currentEnergy;
            _currentEnergy = Mathf.Min(_maxEnergy, _currentEnergy + amount);

            return _currentEnergy - before;
        }

        public int TryRelease()
        {
            if (_currentEnergy <= 0 || _currentEnergy < _maxEnergy)
                return 0;

            var released = _currentEnergy;
            _currentEnergy = 0;

            return released;
        }

        public void Reset()
        {
            _currentEnergy = 0;
        }
    }
}