using Project.Scripts.Services.Events;
using Project.Scripts.Shared.Heroes;

namespace Project.Scripts.Services.Combat
{
    public class AbilityExecutionService : IAbilityExecutionService
    {
        private readonly IPlayerAvatarChargeService _playerAvatarCharge;
        private readonly IHeroService _heroService;
        private readonly IPlayerStateService _playerState;
        private readonly IEnemyStateService _enemyState;
        private readonly IAvatarGroupDefenseService _groupDefense;
        private readonly EventBus _eventBus;


        public AbilityExecutionService(
            IPlayerAvatarChargeService playerAvatarCharge,
            IHeroService heroService,
            IPlayerStateService playerState,
            IEnemyStateService enemyState,
            IAvatarGroupDefenseService groupDefense,
            EventBus eventBus)
        {
            _playerAvatarCharge = playerAvatarCharge;
            _heroService = heroService;
            _playerState = playerState;
            _enemyState = enemyState;
            _groupDefense = groupDefense;
            _eventBus = eventBus;
        }


        public void Execute(UnitDescriptor source, UnitDescriptor target)
        {
            if (target.Kind == UnitKind.Avatar
                && source.ActionType == HeroActionType.DealDamage
                && !_groupDefense.IsExposed(target.Side))
                return;

            if (source.ActionType == HeroActionType.HealAlly
                && source.Kind == target.Kind
                && source.Side == target.Side
                && source.SlotIndex == target.SlotIndex)
                return;

            HeroActionType actionType;
            int actionValue;

            if (source.Kind == UnitKind.Avatar)
            {
                if (!_playerAvatarCharge.TryRelease())
                    return;

                actionType = _playerAvatarCharge.AbilityType;
                actionValue = _playerAvatarCharge.AbilityPower;
            }
            else
            {
                if (!_heroService.TryDischargeHero(source.Side, source.SlotIndex, out actionType, out actionValue))
                    return;
            }

            if (actionValue <= 0)
                return;


            ApplyToTarget(target, actionType, actionValue);
            _eventBus.Publish(new AbilityExecutedEvent(source, target, actionType, actionValue));
        }


        private void ApplyToTarget(UnitDescriptor target, HeroActionType actionType, int actionValue)
        {
            if (actionType == HeroActionType.DealDamage)
            {
                if (target.Kind == UnitKind.Avatar)
                    _enemyState.ApplyDamage(actionValue);
                else
                    _heroService.ApplyDamageToHero(target.Side, target.SlotIndex, actionValue);
            }
            else
            {
                if (target.Kind == UnitKind.Avatar)
                    _playerState.Heal(actionValue);
                else
                    _heroService.ApplyHealToHero(target.Side, target.SlotIndex, actionValue);
            }
        }
    }
}