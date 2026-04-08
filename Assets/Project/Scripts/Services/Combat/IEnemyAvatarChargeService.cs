using System.Collections.Generic;
using Project.Scripts.Shared.Tiles;

namespace Project.Scripts.Services.Combat
{
    public interface IEnemyAvatarChargeService
    {
        int CurrentEnergy { get; }
        int MaxEnergy { get; }
        bool IsReady { get; }
        void AddEnergy(int amount);
        void AddEnergyFromCascades(IReadOnlyDictionary<TileKind, int> energyByKind);
        void TriggerAttack();
    }
}