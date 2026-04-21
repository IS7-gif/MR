using Project.Scripts.Shared.BattleFlow;
using UnityEngine;

namespace Project.Scripts.Configs.Battle
{
    [CreateAssetMenu(fileName = "BattleFlowConfig", menuName = "Configs/Battle/Battle Flow Config")]
    public class BattleFlowConfig : ScriptableObject
    {
        [Tooltip("Количество полных раундов до перехода к пост-раундовому состоянию")]
        [SerializeField] private int _roundCount = 2;

        [Tooltip("Длительность Match Phase в секундах")]
        [SerializeField] private float _matchPhaseDuration = 30f;

        [Tooltip("Длительность Hero Phase в секундах")]
        [SerializeField] private float _heroPhaseDuration = 30f;

        [Tooltip("Оставшееся время в секундах, при котором начинается посекундный обратный отсчёт")]
        [SerializeField] private int _countdownThreshold = 5;

        [Tooltip("Правило переноса остатка энергии между раундами")]
        [SerializeField] private EnergyCarryoverMode _energyCarryoverMode = EnergyCarryoverMode.CarryOverBetweenRounds;


        public int RoundCount => _roundCount;
        public float MatchPhaseDuration => _matchPhaseDuration;
        public float HeroPhaseDuration => _heroPhaseDuration;
        public int CountdownThreshold => _countdownThreshold;
        public EnergyCarryoverMode EnergyCarryoverMode => _energyCarryoverMode;
    }
}