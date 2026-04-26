using UnityEngine;

namespace Project.Scripts.Configs.Gameplay
{
    [CreateAssetMenu(fileName = "GameplayScreenLayoutConfig", menuName = "Configs/Gameplay Screen Layout Config")]
    public class GameplayScreenLayoutConfig : ScriptableObject
    {
        [Header("Gameplay Frame")]
        [Tooltip("Ширина reference-разрешения Canvas/GamePlay в pixels.")]
        [Min(1f)]
        [SerializeField] private float _referenceResolutionWidth = 1080f;

        [Tooltip("Высота reference-разрешения Canvas/GamePlay в pixels.")]
        [Min(1f)]
        [SerializeField] private float _referenceResolutionHeight = 1920f;

        [Tooltip("Ширина эталонного gameplay-контейнера. Для 1:2 оставьте 1.")]
        [Min(0.01f)]
        [SerializeField] private float _gameplayAspectWidth = 1f;

        [Tooltip("Высота эталонного gameplay-контейнера. Для 1:2 оставьте 2.")]
        [Min(0.01f)]
        [SerializeField] private float _gameplayAspectHeight = 2f;

        [Header("Camera")]
        [Tooltip("Ортографический размер камеры при эталонном соотношении gameplay-контейнера.")]
        [Min(0.01f)]
        [SerializeField] private float _referenceOrthographicSize = 5f;

        [Header("Safe Area")]
        [Tooltip("Учитывать Screen.safeArea при расчете gameplay-контейнера.")]
        [SerializeField] private bool _useSafeArea = true;

        [Tooltip("Разрешить world-content зоне использовать нижнюю unsafe area. TopBar при этом остается внутри safe area.")]
        [SerializeField] private bool _worldExtendsIntoUnsafeBottomArea = true;

        [Tooltip("Дополнительный внутренний отступ от safe area в reference pixels.")]
        [Min(0f)]
        [SerializeField] private float _safeAreaPadding = 0f;

        [Header("Top Bar")]
        [Tooltip("Высота зоны TopBar внутри gameplay-контейнера в reference pixels.")]
        [Min(0f)]
        [SerializeField] private float _topBarHeight = 80f;

        [Tooltip("Отступ TopBar от верхнего края gameplay-контейнера в reference pixels.")]
        [Min(0f)]
        [SerializeField] private float _topBarTopPadding = 20f;

        [Tooltip("Боковой отступ TopBar внутри gameplay-контейнера в reference pixels.")]
        [Min(0f)]
        [SerializeField] private float _topBarSidePadding = 140f;

        [Tooltip("Зазор между нижним краем TopBar-зоны и верхом world-content зоны в reference pixels.")]
        [Min(0f)]
        [SerializeField] private float _topBarToWorldGap = 0f;

        [Header("World Content")]
        [Tooltip("Отступ world-content зоны от нижнего края gameplay-контейнера в reference pixels.")]
        [Min(0f)]
        [SerializeField] private float _worldBottomPadding = 8f;

        [Tooltip("Боковой отступ world-content зоны внутри gameplay-контейнера в reference pixels.")]
        [Min(0f)]
        [SerializeField] private float _worldSidePadding = 0f;

        [Tooltip("Минимальная доля vertical gaps между world блоками перед тем, как match-доска начнет уменьшаться.")]
        [Range(0f, 1f)]
        [SerializeField] private float _worldStackMinGapScale = 0.25f;

        public float ReferenceResolutionWidth => _referenceResolutionWidth;
        public float ReferenceResolutionHeight => _referenceResolutionHeight;
        public float GameplayAspect => _gameplayAspectWidth / _gameplayAspectHeight;
        public float ReferenceOrthographicSize => _referenceOrthographicSize;
        public bool UseSafeArea => _useSafeArea;
        public bool WorldExtendsIntoUnsafeBottomArea => _worldExtendsIntoUnsafeBottomArea;
        public float SafeAreaPadding => _safeAreaPadding;
        public float TopBarHeight => _topBarHeight;
        public float TopBarTopPadding => _topBarTopPadding;
        public float TopBarSidePadding => _topBarSidePadding;
        public float TopBarToWorldGap => _topBarToWorldGap;
        public float WorldBottomPadding => _worldBottomPadding;
        public float WorldSidePadding => _worldSidePadding;
        public float WorldStackMinGapScale => _worldStackMinGapScale;
    }
}