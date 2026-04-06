using UnityEngine;

namespace Project.Scripts.Configs.Battle
{
    [CreateAssetMenu(fileName = "AvatarCombatConfig", menuName = "Configs/Battle/Avatar Combat Config")]
    public class AvatarCombatConfig : ScriptableObject
    {
        [Tooltip("Максимальный заряд аватара игрока и врага")]
        [SerializeField] private int _maxAvatarCharge = 110;


        public int MaxAvatarCharge => _maxAvatarCharge;
    }
}