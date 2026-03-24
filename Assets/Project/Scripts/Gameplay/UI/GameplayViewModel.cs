using Cysharp.Threading.Tasks;
using Project.Scripts.Configs;
using Project.Scripts.Services.Combat;
using Project.Scripts.Services.EventBusSystem;
using Project.Scripts.Services.EventBusSystem.Events;
using Project.Scripts.Services.UISystem;
using Project.Scripts.Tiles;
using R3;

namespace Project.Scripts.Gameplay.UI
{
    public class GameplayViewModel : BaseViewModel
    {
        private readonly EventBus _eventBus;
        private readonly IEnemyStateService _enemyState;
        private readonly IMoveCounterService _moveCounter;
        private readonly LevelConfig _levelConfig;
        private readonly IEnergyService _energyService;


        public ReactiveProperty<int> LastDamage { get; } = new(0);
        public ReactiveProperty<int> EnemyHP { get; } = new(0);
        public ReactiveProperty<int> MovesLeft { get; } = new(0);
        public int CurrentLevel { get; private set; }
        public ReactiveProperty<float> FireEnergy { get; } = new(0f);
        public ReactiveProperty<float> WaterEnergy { get; } = new(0f);
        public ReactiveProperty<float> NatureEnergy { get; } = new(0f);
        public ReactiveProperty<float> LightEnergy { get; } = new(0f);
        public ReactiveProperty<float> VoidEnergy { get; } = new(0f);


        public GameplayViewModel(EventBus eventBus, IEnemyStateService enemyState, IMoveCounterService moveCounter,
            LevelConfig levelConfig, IEnergyService energyService)
        {
            _eventBus = eventBus;
            _enemyState = enemyState;
            _moveCounter = moveCounter;
            _levelConfig = levelConfig;
            _energyService = energyService;
        }


        protected override UniTask OnInitializeAsync()
        {
            CurrentLevel = _levelConfig.LevelId;
            EnemyHP.Value = _enemyState.CurrentHP;
            MovesLeft.Value = _moveCounter.RemainingMoves;

            Disposables.Add(_eventBus.Subscribe<DamageDealtEvent>(OnDamageDealt));
            Disposables.Add(_eventBus.Subscribe<EnemyHPChangedEvent>(OnEnemyHPChanged));
            Disposables.Add(_eventBus.Subscribe<MoveCountChangedEvent>(OnMoveCountChanged));
            Disposables.Add(_eventBus.Subscribe<EnergyChangedEvent>(OnEnergyChanged));
            
            return UniTask.CompletedTask;
        }

        protected override void OnCleanup()
        {
            LastDamage.Dispose();
            EnemyHP.Dispose();
            MovesLeft.Dispose();
            FireEnergy.Dispose();
            WaterEnergy.Dispose();
            NatureEnergy.Dispose();
            LightEnergy.Dispose();
            VoidEnergy.Dispose();
        }


        private void OnDamageDealt(DamageDealtEvent e) => LastDamage.Value = e.Total;

        private void OnEnemyHPChanged(EnemyHPChangedEvent e) => EnemyHP.Value = e.Current;

        private void OnMoveCountChanged(MoveCountChangedEvent e) => MovesLeft.Value = e.Remaining;

        private void OnEnergyChanged(EnergyChangedEvent e)
        {
            var normalized = e.Max > 0 ? (float)e.Current / e.Max : 0f;
            switch (e.Kind)
            {
                case TileKind.Fire: FireEnergy.Value = normalized; break;
                case TileKind.Water: WaterEnergy.Value = normalized; break;
                case TileKind.Nature: NatureEnergy.Value = normalized; break;
                case TileKind.Light: LightEnergy.Value = normalized; break;
                case TileKind.Void: VoidEnergy.Value = normalized; break;
            }
        }
    }
}