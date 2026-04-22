using System;
using Project.Scripts.Configs.Levels;
using Project.Scripts.Services.Events;
using Project.Scripts.Services.Game;
using Project.Scripts.Shared.Rules;
using Project.Scripts.Shared.Heroes;
using VContainer.Unity;

namespace Project.Scripts.Services.Combat
{
    public class EnemyAvatarChargeService : IEnemyAvatarChargeService, IStartable, IDisposable
    {
        public int ActivationEnergyCost { get; }
        public bool IsReady => _battleSideEnergyService.CanSpend(BattleSide.Enemy, ActivationEnergyCost)
                               && false == _unitActivationCooldownService.IsAvatarOnCooldown(BattleSide.Enemy);
        public HeroActionType AbilityType { get; }
        public int AbilityPower { get; }


        private readonly EventBus _eventBus;
        private readonly IBattleSideEnergyService _battleSideEnergyService;
        private readonly IBattleActionRuntimeService _battleActionRuntimeService;
        private readonly IUnitActivationCooldownService _unitActivationCooldownService;

        public EnemyAvatarChargeService(
            EventBus eventBus,
            LevelConfig levelConfig,
            IBattleSideEnergyService battleSideEnergyService,
            IBattleActionRuntimeService battleActionRuntimeService,
            IUnitActivationCooldownService unitActivationCooldownService)
        {
            _eventBus = eventBus;
            _battleSideEnergyService = battleSideEnergyService;
            _battleActionRuntimeService = battleActionRuntimeService;
            _unitActivationCooldownService = unitActivationCooldownService;
            var config = levelConfig.EnemyAvatarConfig;
            ActivationEnergyCost = config.ActivationEnergyCost;
            AbilityType = config.AbilityType;
            AbilityPower = config.AbilityPower;
        }

        public void Start()
        {
        }

        public void Dispose()
        {
        }


        public void AddEnergy(int amount)
        {
            _battleSideEnergyService.AddEnergy(BattleSide.Enemy, amount);
        }

        public bool TryRelease()
        {
            if (false == _battleActionRuntimeService.Evaluate(BattleActionKind.AvatarActivation).IsAllowed)
                return false;

            if (_unitActivationCooldownService.IsAvatarOnCooldown(BattleSide.Enemy))
                return false;

            if (false == _battleSideEnergyService.TrySpend(BattleSide.Enemy, ActivationEnergyCost))
                return false;

            _unitActivationCooldownService.StartAvatarCooldown(BattleSide.Enemy);
            return true;
        }

        public void TriggerAttack()
        {
            if (false == _battleActionRuntimeService.Evaluate(BattleActionKind.AvatarActivation).IsAllowed)
                return;

            if (_unitActivationCooldownService.IsAvatarOnCooldown(BattleSide.Enemy))
                return;

            if (false == _battleSideEnergyService.TrySpend(BattleSide.Enemy, ActivationEnergyCost))
                return;

            _unitActivationCooldownService.StartAvatarCooldown(BattleSide.Enemy);
            _eventBus.Publish(new EnemyAvatarActivatedEvent(AbilityType, AbilityPower));
        }
    }
}