using DG.Tweening;
using Project.Scripts.Services.Announcements;
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

        [Tooltip("Вертикальное смещение в мировых координатах относительно трансформа AnnouncementAnchor в префабе BattleFieldView; при значении 0 объявление выводится точно в позиции якоря")]
        [SerializeField] private float _verticalWorldOffset;

        [Tooltip("Стиль анимации по умолчанию для обычных объявлений")]
        [SerializeField] private AnnouncementStyle _style = AnnouncementStyle.FlyUp;

        [Tooltip("Во сколько раз увеличивается текст относительно базового scale при исчезновении в стиле ScaleFade")]
        [SerializeField] private float _scaleMultiplier = 2f;

        [Tooltip("Якорь позиции по умолчанию для обычных объявлений")]
        [SerializeField] private AnnouncementAnchorKind _defaultAnchor = AnnouncementAnchorKind.BattleField;

        [Tooltip("Скрывать ли тексты значений энергии на время показа объявления по умолчанию")]
        [SerializeField] private bool _hideEnergyText;

        [Header("Final Countdown")]
        [Tooltip("Цвет текста цифры обратного отсчёта")]
        [SerializeField] private Color _countdownTextColor = Color.white;

        [Tooltip("Длительность в секундах, в течение которой цифра обратного отсчёта остаётся полностью видимой")]
        [SerializeField] private float _countdownDisplayDuration = 0.5f;

        [Tooltip("Стиль анимации исчезновения цифры обратного отсчёта")]
        [SerializeField] private AnnouncementStyle _countdownStyle = AnnouncementStyle.ScaleFade;

        [Tooltip("Базовый масштаб цифры обратного отсчёта в момент появления")]
        [SerializeField] private float _countdownBaseScale = 2f;

        [Tooltip("Длительность в секундах анимации затухания цифры обратного отсчёта")]
        [SerializeField] private float _countdownFadeOutDuration = 0.4f;

        [Tooltip("Во сколько раз увеличивается цифра относительно базового scale при исчезновении в стиле ScaleFade")]
        [SerializeField] private float _countdownScaleMultiplier = 1.5f;


        public GameObject ViewPrefab => _viewPrefab;
        public AnnouncementStyle Style => _style;
        public AnnouncementAnchorKind DefaultAnchor => _defaultAnchor;
        public bool HideEnergyText => _hideEnergyText;
        public Color TextColor => _textColor;
        public float DisplayDuration => _displayDuration;
        public float FadeOutDuration => _fadeOutDuration;
        public float FlyDistance => _flyDistance;
        public float ScaleMultiplier => _scaleMultiplier;
        public Ease FadeOutEase => _fadeOutEase;
        public float VerticalWorldOffset => _verticalWorldOffset;
        public Color CountdownTextColor => _countdownTextColor;
        public float CountdownDisplayDuration => _countdownDisplayDuration;
        public AnnouncementStyle CountdownStyle => _countdownStyle;
        public float CountdownBaseScale => _countdownBaseScale;
        public float CountdownFadeOutDuration => _countdownFadeOutDuration;
        public float CountdownScaleMultiplier => _countdownScaleMultiplier;
    }
}