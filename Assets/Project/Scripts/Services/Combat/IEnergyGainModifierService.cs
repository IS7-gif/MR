using System.Collections.Generic;
using Project.Scripts.Shared.Heroes;
using Project.Scripts.Shared.Tiles;

namespace Project.Scripts.Services.Combat
{
    public interface IEnergyGainModifierService
    {
        float CalculateMatchEnergy(BattleSide side, IReadOnlyDictionary<TileKind, float> energyByKind);
    }
}