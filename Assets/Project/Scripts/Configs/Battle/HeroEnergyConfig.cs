using UnityEngine;

namespace Project.Scripts.Configs.Battle
{
    [CreateAssetMenu(fileName = "HeroEnergyConfig", menuName = "Configs/Battle/Hero Energy Config")]
    public class HeroEnergyConfig : ScriptableObject
    {
        [Tooltip("Максимальный заряд аватара игрока и врага")]
        [SerializeField] private int _maxAvatarCharge = 110;


        public int MaxAvatarCharge => _maxAvatarCharge;
    }
}