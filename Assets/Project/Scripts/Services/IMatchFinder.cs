using System.Collections.Generic;
using Project.Scripts.Tiles;

namespace Project.Scripts.Services
{
    public interface IMatchFinder
    {
        List<MatchResult> FindMatches(TileType[,] grid);
    }
}