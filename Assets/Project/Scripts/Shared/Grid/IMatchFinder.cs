using System.Collections.Generic;
using Project.Scripts.Shared.Tiles;

namespace Project.Scripts.Shared.Grid
{
    public interface IMatchFinder
    {
        List<MatchResult> FindMatches(TileKind[,] grid);
    }
}