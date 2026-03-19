using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Services.Grid
{
    public class MatchResult
    {
        public List<Vector2Int> Positions;
        public int MaxLineLength;
        public bool IsComplex;
    }
}
