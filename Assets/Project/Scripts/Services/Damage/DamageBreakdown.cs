using System.Collections.Generic;
using System.Text;

namespace Project.Scripts.Services.Damage
{
    public readonly struct DamageBreakdown
    {
        public readonly IReadOnlyList<WaveBreakdown> Waves;
        public readonly int BombDamage;
        public readonly int Total;


        public DamageBreakdown(IReadOnlyList<WaveBreakdown> waves, int bombDamage)
        {
            Waves = waves;
            BombDamage = bombDamage;

            var total = bombDamage;
            for (var i = 0; i < waves.Count; i++)
                total += waves[i].Total;

            Total = total;
        }

        public string ToLogString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("[Damage] Move result:");

            for (var i = 0; i < Waves.Count; i++)
            {
                var wave = Waves[i];
                var cascadeTag = wave.CascadeLevel > 1 ? $" cascade x{wave.CascadeMultiplier:F1}" : "";
                var multiTag = wave.HasMultiMatchBonus ? " +multi" : "";
                sb.AppendLine($" Wave {wave.CascadeLevel}: {wave.MatchCount} match(es){cascadeTag}{multiTag} → {wave.Total} dmg");
            }

            if (BombDamage > 0)
                sb.AppendLine($" Bomb: {BombDamage} dmg");

            sb.AppendLine($" TOTAL: {Total} dmg");
            return sb.ToString();
        }
    }
}