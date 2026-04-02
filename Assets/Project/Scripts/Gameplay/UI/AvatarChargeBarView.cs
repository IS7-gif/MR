using DG.Tweening;
using Project.Scripts.Configs;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Gameplay.UI
{
    public class AvatarChargeBarView : MonoBehaviour
    {
        [Header("Fill")]
        [Tooltip("Vertical filled Image - FillMethod=Vertical, FillOrigin=Bottom")]
        [SerializeField] private Image _fill;

        [Header("Ready state")]
        [Tooltip("GameObject shown only when charge is full (e.g. glow overlay). May be null.")]
        [SerializeField] private GameObject _readyIndicator;


        private CompositeDisposable _disposables;
        private Color _baseFillColor;
        private BattleAnimationConfig _config;
        private Tweener _fillTween;


        private void Awake()
        {
            if (_fill)
                _baseFillColor = _fill.color;
        }

        private void OnDestroy()
        {
            _fillTween?.Kill();
            _disposables?.Dispose();
        }


        public void Bind(AvatarChargeBarViewModel viewModel, IReadyPulseCoordinator pulseCoordinator, BattleAnimationConfig config)
        {
            _fillTween?.Kill();
            _disposables?.Dispose();
            _disposables = new CompositeDisposable();
            _config = config;

            if (_fill)
            {
                _fill.fillAmount = viewModel.FillFraction.CurrentValue;

                viewModel.FillFraction
                    .Skip(1)
                    .Subscribe(v =>
                    {
                        _fillTween?.Kill();
                        if (_config)
                            _fillTween = _fill.DOFillAmount(v, _config.EnergyFillDuration).SetEase(_config.EnergyFillEase);
                        else
                            _fill.fillAmount = v;
                    })
                    .AddTo(_disposables);
            }

            viewModel.IsReady
                .Subscribe(full =>
                {
                    if (_readyIndicator)
                        _readyIndicator.SetActive(full);

                    if (!full && _fill)
                        _fill.color = _baseFillColor;
                })
                .AddTo(_disposables);

            pulseCoordinator.Alpha
                .Subscribe(a =>
                {
                    if (_fill && viewModel.IsReady.CurrentValue)
                    {
                        var c = _baseFillColor;
                        c.a = a;
                        _fill.color = c;
                    }
                })
                .AddTo(_disposables);
        }
    }
}