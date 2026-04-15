using System.Collections.Generic;
using Project.Scripts.Shared.Tiles;

namespace Project.Scripts.Shared.Avatar
{
    public readonly struct AvatarEnergyFormula
    {
        public readonly TileKind AvatarKind;
        public readonly float PrimaryMultiplier;
        public readonly float SecondaryMultiplier;


        public AvatarEnergyFormula(TileKind avatarKind, float primaryMultiplier, float secondaryMultiplier)
        {
            AvatarKind = avatarKind;
            PrimaryMultiplier = primaryMultiplier;
            SecondaryMultiplier = secondaryMultiplier;
        }

        public float Calculate(IReadOnlyDictionary<TileKind, float> energyByKind)
        {
            var total = 0f;
            foreach (var pair in energyByKind)
            {
                var multiplier = pair.Key == AvatarKind ? PrimaryMultiplier : SecondaryMultiplier;
                total += pair.Value * multiplier;
            }

            return total;
        }

        public float Calculate(IReadOnlyDictionary<TileKind, float> energyByKind,
            HashSet<TileKind> bonusKinds, float bonusMultiplier)
        {
            var total = 0f;
            foreach (var pair in energyByKind)
            {
                var multiplier = pair.Key == AvatarKind ? PrimaryMultiplier : SecondaryMultiplier;
                if (bonusKinds.Contains(pair.Key))
                    multiplier *= bonusMultiplier;
                total += pair.Value * multiplier;
            }

            return total;
        }
    }
}