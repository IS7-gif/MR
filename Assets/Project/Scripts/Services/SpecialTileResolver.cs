using System.Collections.Generic;
using Project.Scripts.Configs;
using Project.Scripts.Services.Grid;
using Project.Scripts.Tiles;
using UnityEngine;

namespace Project.Scripts.Services
{
    public class SpecialTileResolver
    {
        private readonly SpecialTileConfig _config;


        public SpecialTileResolver(SpecialTileConfig config) => _config = config;


        public Dictionary<Vector2Int, SpecialTileSpawnData> Resolve(List<MatchResult> matches, Vector2Int pivotPosition)
        {
            var result = new Dictionary<Vector2Int, SpecialTileSpawnData>();
            var rules = _config.Rules;

            for (var i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                var entry = FindEntry(match, rules);

                if (null == entry)
                    continue;

                var spawnPos = entry.SpawnPosition == SpecialTileSpawnPosition.MatchCenter
                    ? match.Center
                    : pivotPosition;

                if (result.ContainsKey(spawnPos))
                    continue;

                var payloadKind = entry.TileToSpawn.Kind == TileKind.Storm
                    ? TileKind.None
                    : match.TileKind;

                result[spawnPos] = new SpecialTileSpawnData(entry.TileToSpawn, payloadKind);
            }

            return result;
        }


        private static SpecialTileEntry FindEntry(MatchResult match, SpecialTileEntry[] rules)
        {
            for (var i = 0; i < rules.Length; i++)
            {
                if (false == rules[i].TileToSpawn)
                    continue;

                if (MatchesCondition(rules[i].Condition, match))
                    return rules[i];
            }

            return null;
        }

        private static bool MatchesCondition(SpecialTileCondition condition, MatchResult match)
        {
            return condition switch
            {
                SpecialTileCondition.LShape => match.Shape == MatchShape.LShape,
                SpecialTileCondition.TShape => match.Shape == MatchShape.TShape,
                SpecialTileCondition.Match4 => match.MaxLineLength == 4 && match.Shape is MatchShape.Horizontal or MatchShape.Vertical,
                SpecialTileCondition.Match4Horizontal => match.MaxLineLength == 4 && match.Shape == MatchShape.Horizontal,
                SpecialTileCondition.Match4Vertical => match.MaxLineLength == 4 && match.Shape == MatchShape.Vertical,
                SpecialTileCondition.Match5 => match.MaxLineLength >= 5 && match.Shape is MatchShape.Horizontal or MatchShape.Vertical,
                _ => false
            };
        }
    }
}