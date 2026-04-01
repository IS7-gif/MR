using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Gameplay.UI
{
    public class AvatarChargeBarView : MonoBehaviour
    {
        [Header("Fill")]
        [Tooltip("Vertical filled Image — FillMethod=Vertical, FillOrigin=Bottom")]
        [SerializeField] private Image _fill;

        [Header("Ready state")]
        [Tooltip("GameObject shown only when charge is full (e.g. glow overlay). May be null.")]
        [SerializeField] private GameObject _readyIndicator;

        [Header("Ready pulse")]
        [Tooltip("Duration of one full ready-pulse cycle on the fill image (seconds)")]
        [SerializeField] private float _pulseDuration = 0.6f;

        [Tooltip("Target alpha for the pulse peak (0-1)")]
        [SerializeField] private float _pulseAlpha = 0.6f;


        private CompositeDisposable _disposables;
        private Tween _pulseTween;
        private Color _baseFillColor;


        private void Awake()
        {
            if (_fill)
                _baseFillColor = _fill.color;
        }

        private void OnDestroy()
        {
            StopPulse();
            _disposables?.Dispose();
        }


        public void Bind(AvatarChargeBarViewModel viewModel)
        {
            _disposables?.Dispose();
            _disposables = new CompositeDisposable();

            if (_fill)
            {
                _fill.fillAmount = viewModel.FillFraction.CurrentValue;

                viewModel.FillFraction
                    .Skip(1)
                    .Subscribe(v => _fill.fillAmount = v)
                    .AddTo(_disposables);
            }

            viewModel.IsFull
                .Subscribe(full =>
                {
                    if (_readyIndicator)
                        _readyIndicator.SetActive(full);

                    if (full)
                        StartPulse();
                    else
                        StopPulse();
                })
                .AddTo(_disposables);
        }


        private void StartPulse()
        {
            if (!_fill)
                return;

            StopPulse();

            var peakColor = _baseFillColor;
            peakColor.a = _pulseAlpha;

            _pulseTween = _fill
                .DOColor(peakColor, _pulseDuration * 0.5f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void StopPulse()
        {
            _pulseTween?.Kill();
            _pulseTween = null;

            if (_fill)
                _fill.color = _baseFillColor;
        }
    }
}