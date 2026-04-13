using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Project.Scripts.Configs;
using Project.Scripts.Configs.Battle;
using Project.Scripts.Configs.Board;
using Project.Scripts.Configs.Levels;
using Project.Scripts.Gameplay.Battle.Targeting;
using Project.Scripts.Gameplay.Battle.Units;
using Project.Scripts.Services.Board;
using Project.Scripts.Services.Combat;
using Project.Scripts.Services.Events;
using Project.Scripts.Services.UISystem;
using Project.Scripts.Shared.Heroes;
using Project.Scripts.Shared.Tiles;
using R3;
using UnityEngine;

namespace Project.Scripts.Gameplay.Battle.HUD
{
    public class BattleHUDViewModel : BaseViewModel
    {
        public AvatarSlotViewModel PlayerAvatar { get; private set; }
        public AvatarSlotViewModel EnemyAvatar { get; private set; }
        public HeroSlotViewModel[] PlayerHeroSlots => _playerHeroSlots;
        public HeroSlotViewModel[] EnemyHeroSlots => _enemyHeroSlots;
        public IReadyPulseCoordinator PulseCoordinator { get; }
        public IAbilityExecutionService AbilityExecution { get; }
        public IAvatarGroupDefenseService GroupDefense { get; }
        public string EnemyName => _levelConfig.BotConfig ? _levelConfig.BotConfig.OpponentName : string.Empty;
        public BattleAnimationConfig BattleAnimConfig => _battleAnimationConfig;
        public UnitDeathConfig DeathConfig { get; private set; }
        public float BoardTopWorldY => _boardBounds.BoardTopWorldY;
        public float BoardHalfWidth => _boardBounds.BoardHalfWidth;
        public float BoardCenterX => _boardBounds.BoardCenterX;
        public ReadOnlyReactiveProperty<int> TimerSeconds => _timerSeconds;


        private readonly EventBus _eventBus;
        private readonly IEnemyStateService _enemyState;
        private readonly IPlayerStateService _playerState;
        private readonly BattleViewConfig _battleViewConfig;
        private readonly BattleAnimationConfig _battleAnimationConfig;
        private readonly IHeroService _heroService;
        private readonly TileKindPaletteConfig _palette;
        private readonly LevelConfig _levelConfig;
        private readonly SlotLayoutConfig _slotLayoutConfig;
        private readonly IBoardBoundsProvider _boardBounds;
        private readonly BattleTimerConfig _battleTimerConfig;
        private readonly UnitDeathConfig _unitDeathConfig;
        private HeroSlotViewModel[] _playerHeroSlots;
        private HeroSlotViewModel[] _enemyHeroSlots;
        private readonly ReactiveProperty<int> _timerSeconds;


        public BattleHUDViewModel(
            EventBus eventBus,
            IEnemyStateService enemyState,
            IPlayerStateService playerState,
            BattleViewConfig battleViewConfig,
            BattleAnimationConfig battleAnimationConfig,
            IHeroService heroService,
            TileKindPaletteConfig palette,
            LevelConfig levelConfig,
            SlotLayoutConfig slotLayoutConfig,
            IBoardBoundsProvider boardBounds,
            IReadyPulseCoordinator pulseCoordinator,
            IAbilityExecutionService abilityExecution,
            IAvatarGroupDefenseService groupDefense,
            BattleTimerConfig battleTimerConfig,
            UnitDeathConfig unitDeathConfig)
        {
            _eventBus = eventBus;
            _enemyState = enemyState;
            _playerState = playerState;
            _battleViewConfig = battleViewConfig;
            _battleAnimationConfig = battleAnimationConfig;
            _heroService = heroService;
            _palette = palette;
            _levelConfig = levelConfig;
            _slotLayoutConfig = slotLayoutConfig;
            _boardBounds = boardBounds;
            PulseCoordinator = pulseCoordinator;
            AbilityExecution = abilityExecution;
            GroupDefense = groupDefense;
            _battleTimerConfig = battleTimerConfig;
            _unitDeathConfig = unitDeathConfig;
            _timerSeconds = new ReactiveProperty<int>((int)battleTimerConfig.BattleDuration);
        }


        protected override UniTask OnInitializeAsync()
        {
            DeathConfig = _unitDeathConfig;
            var avatarColor = _palette.GetColor(_slotLayoutConfig.AvatarSlotKind, Color.gray);

            PlayerAvatar = new AvatarSlotViewModel(
                _eventBus,
                BattleSide.Player,
                avatarColor,
                _levelConfig.PlayerAvatarConfig.Portrait,
                _playerState.CurrentHP,
                _playerState.MaxHP,
                _battleAnimationConfig,
                _levelConfig.PlayerAvatarConfig.AbilityType);

            EnemyAvatar = new AvatarSlotViewModel(
                _eventBus,
                BattleSide.Enemy,
                avatarColor,
                _levelConfig.EnemyAvatarConfig.Portrait,
                _enemyState.CurrentHP,
                _enemyState.MaxHP,
                _battleAnimationConfig,
                _levelConfig.EnemyAvatarConfig.AbilityType);

            _playerHeroSlots = CreateHeroSlotViewModels(
                BattleSide.Player,
                _heroService.GetSlots(BattleSide.Player),
                _levelConfig.PlayerHeroes);

            _enemyHeroSlots = CreateHeroSlotViewModels(
                BattleSide.Enemy,
                _heroService.GetSlots(BattleSide.Enemy),
                _levelConfig.EnemyHeroes);

            Disposables.Add(_eventBus.Subscribe<HeroEnergyChangedEvent>(OnHeroEnergyChanged));
            Disposables.Add(_eventBus.Subscribe<HeroHPChangedEvent>(OnHeroHPChanged));
            Disposables.Add(_eventBus.Subscribe<HeroDefeatedEvent>(OnHeroDefeated));
            Disposables.Add(_eventBus.Subscribe<BattleTimerChangedEvent>(OnBattleTimerChanged));

            return UniTask.CompletedTask;
        }

        protected override void OnCleanup()
        {
            _timerSeconds.Dispose();
            PlayerAvatar?.Dispose();
            EnemyAvatar?.Dispose();

            if (null != _playerHeroSlots)
                for (var i = 0; i < _playerHeroSlots.Length; i++)
                    _playerHeroSlots[i]?.Dispose();

            if (null != _enemyHeroSlots)
                for (var i = 0; i < _enemyHeroSlots.Length; i++)
                    _enemyHeroSlots[i]?.Dispose();
        }


        private void OnHeroEnergyChanged(HeroEnergyChangedEvent e)
        {
            var slots = e.Side == BattleSide.Player ? _playerHeroSlots : _enemyHeroSlots;
            if (null == slots || e.SlotIndex < 0 || e.SlotIndex >= slots.Length)
                return;

            slots[e.SlotIndex]?.UpdateEnergy(e.Current, e.Max);
        }

        private void OnHeroHPChanged(HeroHPChangedEvent e)
        {
            var slots = e.Side == BattleSide.Player ? _playerHeroSlots : _enemyHeroSlots;
            if (null == slots || e.SlotIndex < 0 || e.SlotIndex >= slots.Length)
                return;

            slots[e.SlotIndex]?.UpdateHP(e.Current, e.Max, e.Silent);
        }

        private void OnHeroDefeated(HeroDefeatedEvent e)
        {
            // Reserved for future side effects (sound, particles, etc.).
        }

        private void OnBattleTimerChanged(BattleTimerChangedEvent e)
        {
            _timerSeconds.Value = (int)e.TimeRemaining;
        }

        private HeroSlotViewModel[] CreateHeroSlotViewModels(
            BattleSide side,
            IReadOnlyList<HeroSlotState> states,
            HeroConfig[] configs)
        {
            var slots = new HeroSlotViewModel[4];

            for (var i = 0; i < 4; i++)
            {
                var state = states[i];
                var config = (null != configs && i < configs.Length) ? configs[i] : null;
                var color = state.IsAssigned
                    ? _palette.GetColor(state.SlotKind, Color.gray)
                    : Color.gray;
                var portrait = config ? config.Portrait : null;

                slots[i] = new HeroSlotViewModel(i, side, state, color, portrait);
            }

            return slots;
        }
    }
}