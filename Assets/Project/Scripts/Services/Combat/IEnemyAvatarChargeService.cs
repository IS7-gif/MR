using System.Collections.Generic;
using Project.Scripts.Shared.Heroes;
using Project.Scripts.Shared.Tiles;

namespace Project.Scripts.Services.Combat
{
    public interface IEnemyAvatarChargeService
    {
        int CurrentEnergy { get; }
        int MaxEnergy { get; }
        bool IsReady { get; }
        HeroActionType AbilityType { get; }
        int AbilityPower { get; }
        bool TryRelease();
        void AddEnergy(int amount);
        void AddEnergyFromCascades(IReadOnlyDictionary<TileKind, int> energyByKind);
        void TriggerAttack();
    }
}