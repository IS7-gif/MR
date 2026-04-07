using UnityEngine;

namespace Project.Scripts.Shared.Avatar
{
    public sealed class AvatarEnergyEngine
    {
        public AvatarEnergyState Snapshot => new AvatarEnergyState((int)_currentEnergy, _maxEnergy);


        private int _maxEnergy;
        private float _currentEnergy;


        public void Initialize(int maxEnergy)
        {
            if (maxEnergy <= 0)
            {
                Debug.LogError($"[AvatarEnergyEngine] maxEnergy must be > 0, got {maxEnergy}. Clamping to 1.");
                maxEnergy = 1;
            }

            _maxEnergy = maxEnergy;
            _currentEnergy = 0f;
        }

        public float TryAddEnergy(float amount)
        {
            if (amount <= 0f || _currentEnergy >= _maxEnergy)
                return 0f;

            var before = _currentEnergy;
            _currentEnergy = Mathf.Min(_maxEnergy, _currentEnergy + amount);

            return _currentEnergy - before;
        }

        public int TryRelease()
        {
            if (_currentEnergy < _maxEnergy)
                return 0;

            var released = (int)_currentEnergy;
            _currentEnergy = 0f;

            return released;
        }

        public void Reset()
        {
            _currentEnergy = 0f;
        }
    }
}