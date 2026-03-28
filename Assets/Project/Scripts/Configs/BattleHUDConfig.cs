using UnityEngine;

namespace Project.Scripts.Configs
{
    [CreateAssetMenu(fileName = "BattleHUDConfig", menuName = "Configs/BattleHUD Config")]
    public class BattleHUDConfig : ScriptableObject
    {
        [Tooltip("Portrait sprite shown for the player avatar (null = no sprite)")]
        [SerializeField] private Sprite _playerAvatarSprite;

        [Tooltip("Portrait sprite shown for the enemy avatar (null = no sprite)")]
        [SerializeField] private Sprite _enemyAvatarSprite;

        public Sprite PlayerAvatarSprite => _playerAvatarSprite;
        public Sprite EnemyAvatarSprite => _enemyAvatarSprite;
    }
}