using System.Collections.Generic;
using Project.Scripts.Shared.Damage;
using Project.Scripts.Shared.Grid;

namespace Project.Scripts.Services.Damage
{
    public interface IDamageCalculator
    {
        WaveBreakdown CalculateWave(List<MatchResult> matches, int cascadeLevel);
        int CalculateBombDamage(int tilesDestroyed);
    }
}
