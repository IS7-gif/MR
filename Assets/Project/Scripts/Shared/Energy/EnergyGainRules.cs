using System.Collections.Generic;
using Project.Scripts.Shared.Tiles;

namespace Project.Scripts.Shared.Energy
{
    public static class EnergyGainRules
    {
        public static float SumAll(IReadOnlyDictionary<TileKind, float> energyByKind)
        {
            if (energyByKind == null)
                return 0f;

            var total = 0f;
            foreach (var pair in energyByKind)
            {
                if (pair.Value <= 0f)
                    continue;

                total += pair.Value;
            }

            return total;
        }
    }
}
