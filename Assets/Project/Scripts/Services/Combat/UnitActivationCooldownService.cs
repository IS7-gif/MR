using Project.Scripts.Configs.Levels;
using Project.Scripts.Services.Events;
using Project.Scripts.Shared.Heroes;

namespace Project.Scripts.Services.Combat
{
    public class UnitActivationCooldownService : IUnitActivationCooldownService
    {
        private const int SlotCount = 4;
        private const float MinPublishDelta = 0.01f;


        private readonly EventBus _eventBus;
        private readonly float[] _playerHeroDurations = new float[SlotCount];
        private readonly float[] _enemyHeroDurations = new float[SlotCount];
        private readonly float[] _playerHeroRemaining = new float[SlotCount];
        private readonly float[] _enemyHeroRemaining = new float[SlotCount];
        private readonly float[] _playerHeroLastPublished = new float[SlotCount];
        private readonly float[] _enemyHeroLastPublished = new float[SlotCount];
        private readonly float _playerAvatarDuration;
        private readonly float _enemyAvatarDuration;
        private float _playerAvatarRemaining;
        private float _enemyAvatarRemaining;
        private float _playerAvatarLastPublished = -1f;
        private float _enemyAvatarLastPublished = -1f;


        public UnitActivationCooldownService(EventBus eventBus, LevelConfig levelConfig)
        {
            _eventBus = eventBus;
            FillHeroDurations(_playerHeroDurations, levelConfig.PlayerHeroes);
            FillHeroDurations(_enemyHeroDurations, levelConfig.EnemyHeroes);
            _playerAvatarDuration = levelConfig.PlayerAvatarConfig ? levelConfig.PlayerAvatarConfig.ActivationCooldownSeconds : 0f;
            _enemyAvatarDuration = levelConfig.EnemyAvatarConfig ? levelConfig.EnemyAvatarConfig.ActivationCooldownSeconds : 0f;
        }


        public void Tick(float deltaTime)
        {
            if (deltaTime <= 0f)
                return;

            TickHeroSide(BattleSide.Player, _playerHeroRemaining, _playerHeroDurations, _playerHeroLastPublished, deltaTime);
            TickHeroSide(BattleSide.Enemy, _enemyHeroRemaining, _enemyHeroDurations, _enemyHeroLastPublished, deltaTime);
            TickAvatar(BattleSide.Player, _playerAvatarDuration, ref _playerAvatarRemaining, ref _playerAvatarLastPublished, deltaTime);
            TickAvatar(BattleSide.Enemy, _enemyAvatarDuration, ref _enemyAvatarRemaining, ref _enemyAvatarLastPublished, deltaTime);
        }

        public bool IsHeroOnCooldown(BattleSide side, int slotIndex)
        {
            if (slotIndex is < 0 or >= SlotCount)
                return false;

            return GetHeroRemaining(side)[slotIndex] > 0f;
        }

        public bool IsAvatarOnCooldown(BattleSide side)
        {
            return side == BattleSide.Player ? _playerAvatarRemaining > 0f : _enemyAvatarRemaining > 0f;
        }

        public void StartHeroCooldown(BattleSide side, int slotIndex)
        {
            if (slotIndex is < 0 or >= SlotCount)
                return;

            var durations = GetHeroDurations(side);
            var remaining = GetHeroRemaining(side);
            var duration = durations[slotIndex];
            if (duration <= 0f)
                return;

            remaining[slotIndex] = duration;
            PublishHeroCooldown(side, slotIndex, duration, duration);
        }

        public void StartAvatarCooldown(BattleSide side)
        {
            if (side == BattleSide.Player)
            {
                if (_playerAvatarDuration <= 0f)
                    return;

                _playerAvatarRemaining = _playerAvatarDuration;
                PublishAvatarCooldown(BattleSide.Player, _playerAvatarRemaining, _playerAvatarDuration);
                return;
            }

            if (_enemyAvatarDuration <= 0f)
                return;

            _enemyAvatarRemaining = _enemyAvatarDuration;
            PublishAvatarCooldown(BattleSide.Enemy, _enemyAvatarRemaining, _enemyAvatarDuration);
        }


        private void TickHeroSide(BattleSide side, float[] remaining, float[] durations, float[] lastPublished, float deltaTime)
        {
            for (var i = 0; i < SlotCount; i++)
            {
                if (remaining[i] <= 0f)
                    continue;

                remaining[i] -= deltaTime;
                if (remaining[i] < 0f)
                    remaining[i] = 0f;

                if (lastPublished[i] < 0f || Abs(lastPublished[i] - remaining[i]) >= MinPublishDelta || remaining[i] <= 0f)
                {
                    lastPublished[i] = remaining[i];
                    PublishHeroCooldown(side, i, remaining[i], durations[i]);
                }
            }
        }

        private void TickAvatar(BattleSide side, float duration, ref float remaining, ref float lastPublished, float deltaTime)
        {
            if (remaining <= 0f)
                return;

            remaining -= deltaTime;
            if (remaining < 0f)
                remaining = 0f;

            if (lastPublished < 0f || Abs(lastPublished - remaining) >= MinPublishDelta || remaining <= 0f)
            {
                lastPublished = remaining;
                PublishAvatarCooldown(side, remaining, duration);
            }
        }

        private void PublishHeroCooldown(BattleSide side, int slotIndex, float remaining, float duration)
        {
            _eventBus.Publish(new HeroCooldownChangedEvent(side, slotIndex, remaining, duration));
        }

        private void PublishAvatarCooldown(BattleSide side, float remaining, float duration)
        {
            _eventBus.Publish(new AvatarCooldownChangedEvent(side, remaining, duration));
        }

        private float[] GetHeroRemaining(BattleSide side)
        {
            return side == BattleSide.Player ? _playerHeroRemaining : _enemyHeroRemaining;
        }

        private float[] GetHeroDurations(BattleSide side)
        {
            return side == BattleSide.Player ? _playerHeroDurations : _enemyHeroDurations;
        }

        private static void FillHeroDurations(float[] target, Project.Scripts.Configs.Battle.HeroConfig[] configs)
        {
            for (var i = 0; i < target.Length; i++)
                target[i] = configs != null && i < configs.Length && configs[i] ? configs[i].ActivationCooldownSeconds : 0f;
        }

        private static float Abs(float value)
        {
            return value < 0f ? -value : value;
        }
    }
}
