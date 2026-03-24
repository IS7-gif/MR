using System.Collections.Generic;
using Project.Scripts.Shared.Tiles;

namespace Project.Scripts.Services.Combat
{
    public interface IEnergyService
    {
        IReadOnlyDictionary<TileKind, int> CurrentEnergy { get; }
        int MaxEnergy { get; }
    }
}