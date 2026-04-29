using System;
using System.Collections.Generic;
using Project.Scripts.Services.Events;
using Project.Scripts.Shared.Heroes;
using Project.Scripts.Shared.Passives;
using VContainer.Unity;

namespace Project.Scripts.Services.Combat
{
    public class PassiveActionEffectService : IStartable, IDisposable
    {
        private const int SlotCount = 4;


        private readonly EventBus _eventBus;
        private readonly IHeroService _heroService;
        private readonly IPlayerStateService _playerState;
        private readonly IEnemyStateService _enemyState;
        private readonly IPendingAttackBonusService _pendingAttackBonusService;
        private readonly Random _random = new(0);
        private IDisposable _passiveActivatedSubscription;


        public PassiveActionEffectService(
            EventBus eventBus,
            IHeroService heroService,
            IPlayerStateService playerState,
            IEnemyStateService enemyState,
            IPendingAttackBonusService pendingAttackBonusService)
        {
            _eventBus = eventBus;
            _heroService = heroService;
            _playerState = playerState;
            _enemyState = enemyState;
            _pendingAttackBonusService = pendingAttackBonusService;
        }

        public void Start()
        {
            _passiveActivatedSubscription = _eventBus.Subscribe<HeroPassiveActivatedEvent>(OnHeroPassiveActivated);
        }

        public void Dispose()
        {
            _passiveActivatedSubscription?.Dispose();
            _passiveActivatedSubscription = null;
        }


        private void OnHeroPassiveActivated(HeroPassiveActivatedEvent e)
        {
            var effects = e.State.Definition.ActionEffects;
            for (var i = 0; i < effects.Count; i++)
            {
                var effect = effects[i];
                if (effect.Kind == PassiveActionEffectKind.GrantAttackBonusUntilNextAttack)
                    GrantAttackBonus(e.State, effect);
            }
        }

        private void GrantAttackBonus(HeroPassiveRuntimeState state, PassiveActionEffectDefinition effect)
        {
            var amount = (int)Math.Ceiling(effect.Value);
            if (amount == 0)
                return;

            var owner = UnitDescriptor.Hero(state.Side, state.SlotIndex, HeroActionType.DealDamage);
            var targets = PassiveUnitTargetingRules.SelectTargets(
                effect.Target,
                owner,
                CollectCandidates(),
                _random);

            _pendingAttackBonusService.Grant(targets, amount);
        }

        private List<PassiveUnitTargetCandidate> CollectCandidates()
        {
            var result = new List<PassiveUnitTargetCandidate>(10)
            {
                new(UnitDescriptor.Avatar(BattleSide.Player, HeroActionType.DealDamage),
                    _playerState.CurrentHP,
                    _playerState.MaxHP,
                    _playerState.CurrentHP > 0),
                new(UnitDescriptor.Avatar(BattleSide.Enemy, HeroActionType.DealDamage),
                    _enemyState.CurrentHP,
                    _enemyState.MaxHP,
                    _enemyState.CurrentHP > 0)
            };

            AddHeroCandidates(result, BattleSide.Player);
            AddHeroCandidates(result, BattleSide.Enemy);
            return result;
        }

        private void AddHeroCandidates(List<PassiveUnitTargetCandidate> result, BattleSide side)
        {
            var slots = _heroService.GetSlots(side);
            for (var i = 0; i < SlotCount && i < slots.Count; i++)
            {
                var slot = slots[i];
                result.Add(new PassiveUnitTargetCandidate(UnitDescriptor.Hero(side, i, slot.ActionType),
                    slot.CurrentHP, slot.MaxHP, slot is { IsAssigned: true, IsAlive: true }));
            }
        }
    }
}