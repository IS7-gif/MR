using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Project.Scripts.Configs.Battle;
using Project.Scripts.Services.Combat;
using Project.Scripts.Services.Events;
using Project.Scripts.Services.Game;
using Project.Scripts.Shared.Bot;
using Project.Scripts.Shared.Heroes;
using Project.Scripts.Shared.Tiles;
using R3;
using UnityEngine;
using VContainer.Unity;

namespace Project.Scripts.Services.Bot
{
    public class BotOpponentService : IBotOpponentService, IStartable, IDisposable
    {
        private readonly EventBus _eventBus;
        private readonly IHeroService _heroService;
        private readonly IGameStateService _gameStateService;
        private readonly IEnemyAvatarChargeService _enemyChargeService;
        private readonly IEnemyStateService _enemyState;
        private readonly BotConfig _botConfig;
        private readonly SlotLayoutConfig _slotLayoutConfig;

        private BotDecisionEngine _engine;
        private CancellationTokenSource _cts;
        private IDisposable _stateSub;
        private readonly bool[] _heroActivationPending = new bool[4];
        private bool _dischargeScheduled;


        public BotOpponentService(
            EventBus eventBus,
            IHeroService heroService,
            IGameStateService gameStateService,
            IEnemyAvatarChargeService enemyChargeService,
            IEnemyStateService enemyState,
            BotConfig botConfig,
            SlotLayoutConfig slotLayoutConfig)
        {
            _eventBus = eventBus;
            _heroService = heroService;
            _gameStateService = gameStateService;
            _enemyChargeService = enemyChargeService;
            _enemyState = enemyState;
            _botConfig = botConfig;
            _slotLayoutConfig = slotLayoutConfig;
        }


        public void Start()
        {
            if (false == _botConfig.Enabled)
                return;

            _engine = new BotDecisionEngine(_botConfig.ToSettings(), UnityEngine.Random.Range(0, int.MaxValue));

            _cts = new CancellationTokenSource();

            _stateSub = _gameStateService.State
                .Where(s => s != GameState.Playing)
                .Take(1)
                .Subscribe(_ => StopLoops());

            RunEnemyChargeLoop(_cts.Token).Forget();
            RunHeroEnergyLoop(_cts.Token).Forget();
        }

        public void Dispose()
        {
            StopLoops();
            _stateSub?.Dispose();
            _stateSub = null;
        }


        private async UniTaskVoid RunEnemyChargeLoop(CancellationToken ct)
        {
            while (false == ct.IsCancellationRequested)
            {
                var cancelled = await UniTask
                    .Delay(TimeSpan.FromSeconds(_botConfig.EnemyChargeTickInterval), cancellationToken: ct)
                    .SuppressCancellationThrow();

                if (cancelled || false == _gameStateService.IsPlaying)
                    return;

                var energyByKind = GenerateRandomCascadeEnergy();
                _enemyChargeService.AddEnergyFromCascades(energyByKind);

                if (_enemyChargeService.IsReady && false == _dischargeScheduled)
                {
                    _dischargeScheduled = true;
                    ScheduleDischarge(ct).Forget();
                }
            }
        }

        private float RollCascadeMultiplier()
        {
            var roll = UnityEngine.Random.value;
            if (roll < _botConfig.GreatCascadeChance)
                return _botConfig.GreatCascadeMultiplier;
            if (roll < _botConfig.GoodCascadeChance)
                return _botConfig.GoodCascadeMultiplier;
            return 1f;
        }

        private Dictionary<TileKind, float> GenerateRandomCascadeEnergy()
        {
            var energyByKind = new Dictionary<TileKind, float>();

            var variation = 1f + UnityEngine.Random.Range(-_botConfig.CascadeVariation, _botConfig.CascadeVariation);
            var baseEnergy = _botConfig.BaseEnergyPerTick * variation * RollCascadeMultiplier();

            var availableTypes = new[]
            {
                TileKind.Fire,
                TileKind.Water,
                TileKind.Nature,
                TileKind.Light,
                TileKind.Void
            };

            if (UnityEngine.Random.value < _botConfig.PrimaryTileProbability)
                energyByKind[_slotLayoutConfig.AvatarSlotKind] = baseEnergy;

            var otherCount = UnityEngine.Random.Range(1, 4);
            var shuffledTypes = ShuffleArray(availableTypes);

            for (var i = 0; i < shuffledTypes.Length; i++)
            {
                var tileType = shuffledTypes[i];
                if (tileType == _slotLayoutConfig.AvatarSlotKind)
                    continue;

                if (energyByKind.Count >= otherCount)
                    break;

                energyByKind[tileType] = baseEnergy * UnityEngine.Random.Range(0.3f, 0.8f);
            }

            return energyByKind;
        }

        private T[] ShuffleArray<T>(T[] array)
        {
            var shuffled = (T[])array.Clone();
            for (int i = 0; i < shuffled.Length; i++)
            {
                var randomIndex = UnityEngine.Random.Range(i, shuffled.Length);
                (shuffled[i], shuffled[randomIndex]) = (shuffled[randomIndex], shuffled[i]);
            }
            
            return shuffled;
        }

        private async UniTaskVoid ScheduleDischarge(CancellationToken ct)
        {
            var delay = _engine.GenerateDischargeDelay();

            var cancelled = await UniTask
                .Delay(TimeSpan.FromSeconds(delay), cancellationToken: ct)
                .SuppressCancellationThrow();

            _dischargeScheduled = false;

            if (cancelled || false == _gameStateService.IsPlaying)
                return;

            if (_enemyChargeService.AbilityType == HeroActionType.DealDamage)
            {
                _enemyChargeService.TriggerAttack();
                return;
            }

            var slots = _heroService.GetSlots(BattleSide.Enemy);
            var targetIndex = _engine.PickMostWoundedHero(slots);
            if (targetIndex < 0)
                return;

            if (_enemyChargeService.TryRelease())
            {
                _heroService.ApplyHealToHero(BattleSide.Enemy, targetIndex, _enemyChargeService.AbilityPower);
                var source = UnitDescriptor.Avatar(BattleSide.Enemy, HeroActionType.HealAlly);
                var target = UnitDescriptor.Hero(BattleSide.Enemy, targetIndex, HeroActionType.HealAlly);
                _eventBus.Publish(new AbilityExecutedEvent(source, target, HeroActionType.HealAlly, _enemyChargeService.AbilityPower));
            }
        }

        private async UniTaskVoid RunHeroEnergyLoop(CancellationToken ct)
        {
            while (false == ct.IsCancellationRequested)
            {
                var cancelled = await UniTask
                    .Delay(TimeSpan.FromSeconds(_botConfig.HeroEnergyTickInterval), cancellationToken: ct)
                    .SuppressCancellationThrow();

                if (cancelled || false == _gameStateService.IsPlaying)
                    return;

                var slots = _heroService.GetSlots(BattleSide.Enemy);
                var pickedIndex = _engine.PickRandomAssignedSlot(slots);

                if (pickedIndex < 0)
                    continue;

                var energyAmount = Mathf.RoundToInt(_botConfig.HeroEnergyPerTick * RollCascadeMultiplier());
                _heroService.AddEnemyHeroEnergy(pickedIndex, energyAmount);

                var updatedSlot = _heroService.GetSlots(BattleSide.Enemy)[pickedIndex];
                if (updatedSlot.IsReady && false == _heroActivationPending[pickedIndex])
                {
                    if (updatedSlot.ActionType == HeroActionType.HealAlly
                        && _enemyState.CurrentHP >= _enemyState.MaxHP)
                        continue;

                    _heroActivationPending[pickedIndex] = true;
                    ActivateWithDelay(pickedIndex, ct).Forget();
                }
            }
        }

        private async UniTaskVoid ActivateWithDelay(int slotIndex, CancellationToken ct)
        {
            var delay = _engine.GenerateDelay(
                _botConfig.MinHeroActivationDelay,
                _botConfig.MaxHeroActivationDelay);

            var cancelled = await UniTask
                .Delay(TimeSpan.FromSeconds(delay), cancellationToken: ct)
                .SuppressCancellationThrow();

            _heroActivationPending[slotIndex] = false;

            if (cancelled || false == _gameStateService.IsPlaying)
                return;

            _heroService.TryActivate(BattleSide.Enemy, slotIndex);
        }

        private void StopLoops()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }
    }
}