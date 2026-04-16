using System;
using DG.Tweening;
using UnityEngine;

namespace Project.Scripts.Configs.UI
{
    [CreateAssetMenu(fileName = "GameResultSequenceConfig", menuName = "Configs/UI/Game Result Sequence Config")]
    public class GameResultSequenceConfig : ScriptableObject
    {
        [Tooltip("Конфигурация последовательности показа результата победы.")]
        [SerializeField] private WinResultSequenceConfig _winSequence = new();

        [Space(20)]
        [Tooltip("Конфигурация последовательности показа результата поражения.")]
        [SerializeField] private LoseResultSequenceConfig _loseSequence = new();


        public WinResultSequenceConfig WinSequence => _winSequence;
        public LoseResultSequenceConfig LoseSequence => _loseSequence;
    }

    [Serializable]
    public class AvatarPulseStepConfig
    {
        [Tooltip("Должен ли выполняться этот шаг последовательности.")]
        [SerializeField] private bool _enabled = true;

        [Tooltip("Множитель масштаба, применяемый в пике анимации пульсации.")]
        [SerializeField] private float _scaleMultiplier = 1.12f;

        [Tooltip("Общая длительность анимации пульсации в секундах.")]
        [SerializeField] private float _duration = 0.35f;

        [Tooltip("Ease, используемый для анимации пульсации.")]
        [SerializeField] private Ease _ease = Ease.InOutSine;

        [Tooltip("Задержка в секундах после завершения этого шага.")]
        [SerializeField] private float _delayAfterStep;


        public bool Enabled => _enabled;
        public float ScaleMultiplier => _scaleMultiplier;
        public float Duration => _duration;
        public Ease Ease => _ease;
        public float DelayAfterStep => _delayAfterStep;
    }

    [Serializable]
    public class AnnouncementStepConfig
    {
        [Tooltip("Должен ли выполняться этот шаг последовательности.")]
        [SerializeField] private bool _enabled = true;

        [Tooltip("Задержка в секундах после завершения этого шага.")]
        [SerializeField] private float _delayAfterStep;


        public bool Enabled => _enabled;
        public float DelayAfterStep => _delayAfterStep;
    }

    [Serializable]
    public class PopupStepConfig
    {
        [Tooltip("Должен ли выполняться этот шаг последовательности.")]
        [SerializeField] private bool _enabled = true;

        [Tooltip("Задержка в секундах после завершения этого шага.")]
        [SerializeField] private float _delayAfterStep;


        public bool Enabled => _enabled;
        public float DelayAfterStep => _delayAfterStep;
    }

    [Serializable]
    public class WinResultSequenceConfig
    {
        [Tooltip("Задержка в секундах перед началом первого шага победы.")]
        [SerializeField] private float _delayBeforeFirstStep = 1f;

        [Space(10)]
        [Tooltip("Шаг анимации пульсации для портрета победившего аватара.")]
        [SerializeField] private AvatarPulseStepConfig _winnerAvatarPulse = new();

        [Tooltip("Шаг объявления, показываемый при Flawless победах.")]
        [SerializeField] private AnnouncementStepConfig _showFlawlessAnnouncement = new();

        [Tooltip("Шаг всплывающего окна для окна победы.")]
        [SerializeField] private PopupStepConfig _showWinPopup = new();


        public float DelayBeforeFirstStep => _delayBeforeFirstStep;
        public AvatarPulseStepConfig WinnerAvatarPulse => _winnerAvatarPulse;
        public AnnouncementStepConfig ShowFlawlessAnnouncement => _showFlawlessAnnouncement;
        public PopupStepConfig ShowWinPopup => _showWinPopup;
    }

    [Serializable]
    public class LoseResultSequenceConfig
    {
        [Tooltip("Задержка в секундах перед началом первого шага поражения.")]
        [SerializeField] private float _delayBeforeFirstStep = 1f;

        [Space(10)]
        [Tooltip("Шаг всплывающего окна для окна поражения.")]
        [SerializeField] private PopupStepConfig _showLosePopup = new();


        public float DelayBeforeFirstStep => _delayBeforeFirstStep;
        public PopupStepConfig ShowLosePopup => _showLosePopup;
    }
}