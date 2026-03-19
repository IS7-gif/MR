using System.Collections.Generic;
using Project.Scripts.Configs;
using Project.Scripts.Services.Grid;
using UnityEngine;

namespace Project.Scripts.Services.Damage
{
    public class DamageCalculator : IDamageCalculator
    {
        private readonly DamageConfig _config;


        public DamageCalculator(DamageConfig config)
        {
            _config = config;
        }

        public WaveBreakdown CalculateWave(List<MatchResult> matches, int cascadeLevel)
        {
            var rawDamage = 0;
            for (var i = 0; i < matches.Count; i++)
                rawDamage += GetMatchBaseDamage(matches[i].MaxLineLength);

            var multiplier = 1f + _config.CascadeMultiplierStep * (cascadeLevel - 1);
            var multiMatch = matches.Count > 1;

            var total = rawDamage * multiplier;
            if (multiMatch)
                total *= 1f + _config.MultiMatchBonus;

            return new WaveBreakdown(cascadeLevel, matches.Count, rawDamage, multiplier, multiMatch,
                Mathf.RoundToInt(total));
        }

        public int CalculateBombDamage(int tilesDestroyed)
        {
            return tilesDestroyed * _config.BombDamagePerTile;
        }

        private int GetMatchBaseDamage(int matchSize)
        {
            if (matchSize >= 5)
                return _config.Match5PlusDamage + (matchSize - 5) * _config.ExtraTileDamage;
            if (matchSize == 4)
                return _config.Match4Damage;
            return _config.Match3Damage;
        }
    }
}
