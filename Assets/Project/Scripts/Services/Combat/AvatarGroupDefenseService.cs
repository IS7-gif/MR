using System;
using Project.Scripts.Configs.Battle;
using Project.Scripts.Configs.Levels;
using Project.Scripts.Services.Events;
using Project.Scripts.Shared.GroupDefense;
using Project.Scripts.Shared.Heroes;

namespace Project.Scripts.Services.Combat
{
    public class AvatarGroupDefenseService : IAvatarGroupDefenseService, IDisposable
    {
        private const int SlotCount = 4;


        private readonly SlotLayoutConfig _slotLayoutConfig;
        private readonly EventBus _eventBus;

        private readonly bool[] _playerDefending = new bool[SlotCount];
        private readonly bool[] _enemyDefending = new bool[SlotCount];

        private AvatarDefenseSnapshot _playerSnapshot;
        private AvatarDefenseSnapshot _enemySnapshot;

        private IDisposable _subscription;


        public AvatarGroupDefenseService(LevelConfig levelConfig, SlotLayoutConfig slotLayoutConfig,
            EventBus eventBus)
        {
            _slotLayoutConfig = slotLayoutConfig;
            _eventBus = eventBus;

            InitDefending(_playerDefending, levelConfig.PlayerHeroes);
            InitDefending(_enemyDefending, levelConfig.EnemyHeroes);

            _playerSnapshot = ComputeSnapshot(_playerDefending);
            _enemySnapshot = ComputeSnapshot(_enemyDefending);

            _subscription = _eventBus.Subscribe<HeroDefeatedEvent>(OnHeroDefeated);
        }


        public bool IsExposed(BattleSide side)
        {
            return side == BattleSide.Player
                ? _playerSnapshot.IsExposed
                : _enemySnapshot.IsExposed;
        }

        public AvatarDefenseSnapshot GetSnapshot(BattleSide side)
        {
            return side == BattleSide.Player ? _playerSnapshot : _enemySnapshot;
        }

        public void Dispose()
        {
            _subscription?.Dispose();
            _subscription = null;
        }


        private void OnHeroDefeated(HeroDefeatedEvent e)
        {
            var defending = e.Side == BattleSide.Player ? _playerDefending : _enemyDefending;
            var prev = e.Side == BattleSide.Player ? _playerSnapshot : _enemySnapshot;

            if (e.SlotIndex >= 0 && e.SlotIndex < SlotCount)
                defending[e.SlotIndex] = false;

            var next = ComputeSnapshot(defending);

            if (e.Side == BattleSide.Player)
                _playerSnapshot = next;
            else
                _enemySnapshot = next;

            if (next.IsExposed && !prev.IsExposed)
            {
                var groupId = (!prev.IsGroup1Destroyed && next.IsGroup1Destroyed)
                    ? HeroGroupId.Group1
                    : HeroGroupId.Group2;

                _eventBus.Publish(new AvatarExposedEvent(e.Side, groupId));
            }
        }

        private AvatarDefenseSnapshot ComputeSnapshot(bool[] defending)
        {
            var g1 = IsGroupDestroyed(defending, _slotLayoutConfig.Group1SlotIndices);
            var g2 = IsGroupDestroyed(defending, _slotLayoutConfig.Group2SlotIndices);
            
            return new AvatarDefenseSnapshot(g1, g2);
        }

        private static bool IsGroupDestroyed(bool[] defending, int[] groupIndices)
        {
            for (var i = 0; i < groupIndices.Length; i++)
            {
                var idx = groupIndices[i];
                if (idx >= 0 && idx < SlotCount && defending[idx])
                    return false;
            }

            return true;
        }

        private static void InitDefending(bool[] defending, HeroConfig[] configs)
        {
            for (var i = 0; i < SlotCount; i++)
                defending[i] = configs != null && i < configs.Length && configs[i];
        }
    }
}