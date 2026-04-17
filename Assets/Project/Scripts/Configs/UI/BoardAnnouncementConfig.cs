using DG.Tweening;
using UnityEngine;

namespace Project.Scripts.Configs.UI
{
    [CreateAssetMenu(fileName = "BoardAnnouncementConfig", menuName = "Configs/UI/Board Announcement Config")]
    public class BoardAnnouncementConfig : ScriptableObject
    {
        [Tooltip("Префаб BoardAnnouncementView - используется для показа объявлений поверх доски")]
        [SerializeField] private GameObject _viewPrefab;

        [Tooltip("Базовый цвет текста обычного объявления, если он не переопределён в параметрах показа")]
        [SerializeField] private Color _textColor = Color.white;

        [Tooltip("Длительность в секундах, в течение которой обычное объявление остаётся полностью видимым")]
        [SerializeField] private float _displayDuration = 1f;

        [Tooltip("Длительность в секундах анимации затухания обычного объявления")]
        [SerializeField] private float _fadeOutDuration = 0.6f;

        [Tooltip("Расстояние в пикселях, на которое объявление улетает вверх при затухании")]
        [SerializeField] private float _flyDistance = 100f;

        [Tooltip("Кривая ослабления для движения и затухания обычного объявления")]
        [SerializeField] private Ease _fadeOutEase = Ease.InQuad;

        [Tooltip("Вертикальное смещение в мировых координатах относительно трансформа AnnouncementAnchor в префабе BattleHUDView; при значении 0 объявление выводится точно в позиции якоря")]
        [SerializeField] private float _verticalWorldOffset;

        [Tooltip("Цвет текста цифры обратного отсчёта перед овертаймом")]
        [SerializeField] private Color _countdownTextColor = Color.white;

        [Tooltip("Длительность в секундах, в течение которой цифра обратного отсчёта остаётся полностью видимой")]
        [SerializeField] private float _countdownDisplayDuration = 0.5f;

        [Tooltip("Длительность в секундах анимации затухания цифры обратного отсчёта")]
        [SerializeField] private float _countdownFadeOutDuration = 0.25f;

        [Tooltip("Расстояние в пикселях, на которое цифра обратного отсчёта улетает вверх при затухании")]
        [SerializeField] private float _countdownFlyDistance = 60f;

        [Tooltip("Кривая ослабления для движения и затухания цифры обратного отсчёта")]
        [SerializeField] private Ease _countdownFadeOutEase = Ease.InQuad;


        public GameObject ViewPrefab => _viewPrefab;
        public Color TextColor => _textColor;
        public float DisplayDuration => _displayDuration;
        public float FadeOutDuration => _fadeOutDuration;
        public float FlyDistance => _flyDistance;
        public Ease FadeOutEase => _fadeOutEase;
        public float VerticalWorldOffset => _verticalWorldOffset;
        public Color CountdownTextColor => _countdownTextColor;
        public float CountdownDisplayDuration => _countdownDisplayDuration;
        public float CountdownFadeOutDuration => _countdownFadeOutDuration;
        public float CountdownFlyDistance => _countdownFlyDistance;
        public Ease CountdownFadeOutEase => _countdownFadeOutEase;
    }
}