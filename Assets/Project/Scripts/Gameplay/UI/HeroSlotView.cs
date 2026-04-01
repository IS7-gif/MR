using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Gameplay.UI
{
    public class HeroSlotView : MonoBehaviour
    {
        [Header("Visuals")]
        [Tooltip("Background Image")]
        [SerializeField] private Image _background;

        [Tooltip("Hero portrait Image — tinted with the hero's element color when a hero is assigned")]
        [SerializeField] private Image _portrait;

        [Tooltip("Energy bar fill Image (Type=Filled, FillMethod=Vertical, FillOrigin=Bottom)")]
        [SerializeField] private Image _energyBarFill;

        [Header("Interaction")]
        [Tooltip("Button for activating this hero — assign only on player slots; leave null for enemy slots")]
        [SerializeField] private Button _activateButton;

        [Header("Ready Pulse Animation")]
        [Tooltip("Duration of one full pulse cycle in seconds")]
        [SerializeField] private float _pulseDuration = 0.6f;

        [Tooltip("Target alpha for the pulse peak (0-1)")]
        [SerializeField] private float _pulseAlpha = 0.4f;


        private Color _defaultEnergyBarColor;
        private Tween _pulseTween;
        private CompositeDisposable _disposables = new();


        private void Awake()
        {
            if (_energyBarFill)
                _defaultEnergyBarColor = _energyBarFill.color;
        }

        private void OnDestroy()
        {
            StopPulse();

            if (_activateButton)
                _activateButton.onClick.RemoveAllListeners();

            _disposables?.Dispose();
        }


        public void Bind(HeroSlotViewModel viewModel)
        {
            if (_portrait)
            {
                _portrait.enabled = viewModel.IsAssigned;

                if (viewModel.IsAssigned)
                {
                    _portrait.color = viewModel.SlotColor;

                    if (viewModel.Portrait)
                        _portrait.sprite = viewModel.Portrait;
                }
            }

            if (_energyBarFill)
            {
                if (viewModel.IsAssigned)
                {
                    _energyBarFill.color = viewModel.SlotColor;
                    _defaultEnergyBarColor = viewModel.SlotColor;
                }

                _energyBarFill.fillAmount = viewModel.EnergyFill.CurrentValue;

                viewModel.EnergyFill
                    .Skip(1)
                    .Subscribe(v => _energyBarFill.fillAmount = v)
                    .AddTo(_disposables);

                if (viewModel.IsAssigned)
                {
                    viewModel.IsActivatable
                        .Subscribe(activatable =>
                        {
                            if (activatable)
                                StartPulse();
                            else
                                StopPulse();
                        })
                        .AddTo(_disposables);
                }
            }

            if (_activateButton)
            {
                if (viewModel.IsPlayerSlot)
                {
                    viewModel.IsActivatable
                        .Subscribe(activatable => _activateButton.interactable = activatable)
                        .AddTo(_disposables);

                    _activateButton.onClick.AddListener(() => viewModel.OnActivateClicked?.Invoke(viewModel.SlotIndex));
                }
                else
                    _activateButton.enabled = false;
            }
        }


        private void StartPulse()
        {
            if (!_energyBarFill)
                return;

            StopPulse();

            var peakColor = _defaultEnergyBarColor;
            peakColor.a = _pulseAlpha;

            _pulseTween = _energyBarFill
                .DOColor(peakColor, _pulseDuration * 0.5f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void StopPulse()
        {
            _pulseTween?.Kill();
            _pulseTween = null;

            if (_energyBarFill)
                _energyBarFill.color = _defaultEnergyBarColor;
        }
    }
}