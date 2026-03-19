using System.Collections.Generic;
using Project.Scripts.Services.Grid;

namespace Project.Scripts.Services.Damage
{
    public interface IDamageCalculator
    {
        WaveBreakdown CalculateWave(List<MatchResult> matches, int cascadeLevel);
        int CalculateBombDamage(int tilesDestroyed);
    }
}
