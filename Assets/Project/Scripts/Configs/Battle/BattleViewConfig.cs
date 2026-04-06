using UnityEngine;

namespace Project.Scripts.Configs.Battle
{
    [CreateAssetMenu(fileName = "BattleViewConfig", menuName = "Configs/Battle/Battle View Config")]
    public class BattleViewConfig : ScriptableObject
    {
        [Tooltip("Префаб компонента BattleHUDView - корень отображения боя в мировом пространстве")]
        [SerializeField] private GameObject _battleHUDViewPrefab;

        [Tooltip("Смещение по Y в мировых координатах, добавляемое выше верхнего края доски для позиционирования корня боевого HUD")]
        [SerializeField] private float _battleAreaTopPadding = 0.4f;

        [Tooltip("Спрайт портрета аватара игрока (null = без спрайта)")]
        [SerializeField] private Sprite _playerAvatarSprite;

        [Tooltip("Спрайт портрета аватара врага (null = без спрайта)")]
        [SerializeField] private Sprite _enemyAvatarSprite;


        public GameObject BattleHUDViewPrefab => _battleHUDViewPrefab;
        public float BattleAreaTopPadding => _battleAreaTopPadding;
        public Sprite PlayerAvatarSprite => _playerAvatarSprite;
        public Sprite EnemyAvatarSprite => _enemyAvatarSprite;
    }
}