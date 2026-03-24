using UnityEngine;

namespace Project.Scripts.Configs
{
    [CreateAssetMenu(fileName = "EnergyConfig", menuName = "Configs/Energy Config")]
    public class EnergyConfig : ScriptableObject
    {
        [Tooltip("Maximum energy capacity for each tile type")]
        [SerializeField] private int _maxEnergyPerType = 20;


        public int MaxEnergyPerType => _maxEnergyPerType;
    }
}