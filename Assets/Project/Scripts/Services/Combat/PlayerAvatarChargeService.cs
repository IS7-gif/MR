using System;
using Project.Scripts.Configs.Levels;
using Project.Scripts.Services.Game;
using Project.Scripts.Shared.Rules;
using Project.Scripts.Shared.Heroes;
using VContainer.Unity;

namespace Project.Scripts.Services.Combat
{
    public class PlayerAvatarChargeService : IPlayerAvatarChargeService, IStartable, IDisposable
    {
        public int ActivationEnergyCost { get; }
        public bool IsReady => _battleSideEnergyService.CanSpend(BattleSide.Player, ActivationEnergyCost)
                               && false == _unitActivationCooldownService.IsAvatarOnCooldown(BattleSide.Player);
        public HeroActionType AbilityType { get; }
        public int AbilityPower => _abilityPowerModifierService.GetAbilityPower(
            UnitDescriptor.Avatar(BattleSide.Player, AbilityType),
            _baseAbilityPower);

        
        private readonly IBattleSideEnergyService _battleSideEnergyService;
        private readonly IBattleActionRuntimeService _battleActionRuntimeService;
        private readonly IUnitActivationCooldownService _unitActivationCooldownService;
        private readonly IAbilityPowerModifierService _abilityPowerModifierService;
        private readonly int _baseAbilityPower;

        
        public PlayerAvatarChargeService(
            LevelConfig levelConfig,
            IBattleSideEnergyService battleSideEnergyService,
            IBattleActionRuntimeService battleActionRuntimeService,
            IUnitActivationCooldownService unitActivationCooldownService,
            IAbilityPowerModifierService abilityPowerModifierService)
        {
            _battleSideEnergyService = battleSideEnergyService;
            _battleActionRuntimeService = battleActionRuntimeService;
            _unitActivationCooldownService = unitActivationCooldownService;
            _abilityPowerModifierService = abilityPowerModifierService;
            var config = levelConfig.PlayerAvatarConfig;

            ActivationEnergyCost = config.ActivationEnergyCost;
            AbilityType = config.AbilityType;
            _baseAbilityPower = config.AbilityPower;
        }

        public void Start()
        {
        }

        public void Dispose()
        {
        }

        public bool TryRelease()
        {
            if (false == _battleActionRuntimeService.Evaluate(BattleActionKind.AvatarActivation).IsAllowed)
                return false;

            if (_unitActivationCooldownService.IsAvatarOnCooldown(BattleSide.Player))
                return false;

            if (false == _battleSideEnergyService.TrySpend(BattleSide.Player, ActivationEnergyCost))
                return false;

            _unitActivationCooldownService.StartAvatarCooldown(BattleSide.Player);
            
            return true;
        }
    }
}