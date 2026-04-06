using UnityEngine;

namespace Project.Scripts.Configs.Board
{
    [CreateAssetMenu(fileName = "SpecialTileConfig", menuName = "Configs/Special Tile Config")]
    public class SpecialTileConfig : ScriptableObject
    {
        [Tooltip("Правила, определяющие какой специальный тайл создаётся для каждого условия совпадения, проверяются по порядку - выигрывает первое подходящее правило")]
        [SerializeField] private SpecialTileEntry[] _rules;


        public SpecialTileEntry[] Rules => _rules;
    }
}