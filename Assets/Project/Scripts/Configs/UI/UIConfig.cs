using UnityEngine;

namespace Project.Scripts.Configs.UI
{
    [CreateAssetMenu(fileName = "UIConfig", menuName = "Configs/UI Config")]
    public class UIConfig : ScriptableObject
    {
        [Tooltip("Префаб компонента WinView - отображается, когда игрок побеждает врага")]
        [SerializeField] private GameObject _winViewPrefab;

        [Tooltip("Префаб компонента LoseView - отображается, когда у игрока заканчиваются ходы")]
        [SerializeField] private GameObject _loseViewPrefab;

        [Tooltip("Префаб компонента MoveBarView - прикреплён к нижней части экрана")]
        [SerializeField] private GameObject _moveBarViewPrefab;

        [Tooltip("Префаб компонента TopBarView - имя врага и второстепенный ярлык, остаётся в Canvas")]
        [SerializeField] private GameObject _topBarViewPrefab;

        [Tooltip("Prefab for the BoardAnnouncementView - general-purpose battle announcement banner")]
        [SerializeField] private GameObject _boardAnnouncementViewPrefab;


        public GameObject WinViewPrefab => _winViewPrefab;
        public GameObject LoseViewPrefab => _loseViewPrefab;
        public GameObject MoveBarViewPrefab => _moveBarViewPrefab;
        public GameObject TopBarViewPrefab => _topBarViewPrefab;
        public GameObject BoardAnnouncementViewPrefab => _boardAnnouncementViewPrefab;
    }
}