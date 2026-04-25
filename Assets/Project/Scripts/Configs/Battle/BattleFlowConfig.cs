using Project.Scripts.Shared.BattleFlow;
using UnityEngine;

namespace Project.Scripts.Configs.Battle
{
    [CreateAssetMenu(fileName = "BattleFlowConfig", menuName = "Configs/Battle/Battle Flow Config")]
    public class BattleFlowConfig : ScriptableObject
    {
        [Header("Rounds settings")]
        [Tooltip("Количество полных раундов до перехода к пост-раундовому состоянию")]
        [SerializeField] private int _roundCount = 2;

        [Tooltip("Длительность Match Phase в секундах")]
        [SerializeField] private float _matchPhaseDuration = 30f;

        [Tooltip("Длительность Hero Phase в секундах")]
        [SerializeField] private float _heroPhaseDuration = 30f;
        
        [Tooltip("Правило переноса остатка энергии между раундами")]
        [SerializeField] private EnergyCarryoverMode _energyCarryoverMode = EnergyCarryoverMode.CarryOverBetweenRounds;

        [Header("PrePhase settings")]
        [Tooltip("Длительность подготовительной паузы перед фазой в секундах")]
        [SerializeField] private int _prePhaseDuration = 5;

        [Tooltip("Если включено, во время подготовительной паузы затемняются оба экрана фаз")]
        [SerializeField] private bool _dimCurrentPhaseDuringPrePhase = true;

        [Header("Final countdown settings")]
        [Tooltip("Оставшееся время в секундах, при котором начинается посекундный обратный отсчёт")]
        [SerializeField] private int _countdownThreshold = 5;


        public int RoundCount => _roundCount;
        public float MatchPhaseDuration => _matchPhaseDuration;
        public float HeroPhaseDuration => _heroPhaseDuration;
        public int PrePhaseDuration => _prePhaseDuration;
        public bool DimCurrentPhaseDuringPrePhase => _dimCurrentPhaseDuringPrePhase;
        public int CountdownThreshold => _countdownThreshold;
        public EnergyCarryoverMode EnergyCarryoverMode => _energyCarryoverMode;
    }
}