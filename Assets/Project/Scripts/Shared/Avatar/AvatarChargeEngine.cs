using UnityEngine;

namespace Project.Scripts.Shared.Avatar
{
    public sealed class AvatarChargeEngine
    {
        public AvatarChargeState Snapshot => new AvatarChargeState(_currentCharge, _maxCharge);
        
        
        private int _maxCharge;
        private int _currentCharge;


        public void Initialize(int maxCharge)
        {
            if (maxCharge <= 0)
            {
                Debug.LogError($"[AvatarChargeEngine] maxCharge must be > 0, got {maxCharge}. Clamping to 1.");
                maxCharge = 1;
            }

            _maxCharge = maxCharge;
            _currentCharge = 0;
        }

        public int TryAddCharge(int amount)
        {
            if (amount <= 0 || _currentCharge >= _maxCharge)
                return 0;

            var before = _currentCharge;
            _currentCharge = Mathf.Min(_maxCharge, _currentCharge + amount);
            
            return _currentCharge - before;
        }

        public int TryDischarge()
        {
            if (_currentCharge <= 0 || _currentCharge < _maxCharge)
                return 0;

            var discharged = _currentCharge;
            _currentCharge = 0;
            
            return discharged;
        }

        public void Reset()
        {
            _currentCharge = 0;
        }
    }
}