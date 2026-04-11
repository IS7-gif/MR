using DG.Tweening;
using Project.Scripts.Configs.Battle;
using Project.Scripts.Gameplay.Battle.Targeting;
using Project.Scripts.Shared.Heroes;
using R3;
using UnityEngine;

namespace Project.Scripts.Gameplay.Battle.Units
{
    public class HeroSlotView : MonoBehaviour, ITargetable
    {
        [Tooltip("Background SpriteRenderer - tinted with the hero element color; defines slot bounds and is scaled via SetSize")]
        [SerializeField] private SpriteRenderer _background;

        [Tooltip("Portrait SpriteRenderer - displays the character sprite and flashes on hit")]
        [SerializeField] private SpriteRenderer _portrait;

        [Tooltip("SpriteRenderer свечения с материалом Additive - отображается как подсветка источника или цели")]
        [SerializeField] private SpriteRenderer _glow;

        [Tooltip("Основная полоса HP - мгновенно обновляется при получении урона")]
        [SerializeField] private BarRenderer _hpBar;

        [Tooltip("Лаг-полоса HP позади основной полосы - опустошается с задержкой после получения урона")]
        [SerializeField] private BarRenderer _hpLagBar;

        [Tooltip("Полоса энергии - окрашена в цвет элемента героя")]
        [SerializeField] private BarRenderer _energyBar;

        [Tooltip("Якорь для всплывающих чисел урона/лечения - по умолчанию центр слота, если не назначен")]
        [SerializeField] private Transform _hitAnchor;


        public Transform HitAnchor => _hitAnchor ? _hitAnchor : transform;
        public UnitDescriptor Descriptor => UnitDescriptor.Hero(_viewModel.Side, _viewModel.SlotIndex, _viewModel.ActionType);
        public bool IsReadySource => _viewModel != null && _viewModel.IsAssigned && _viewModel.IsActivatable.CurrentValue;
        public Bounds WorldBounds => _background ? _background.bounds : new Bounds(transform.position, Vector3.one);


        private HeroSlotViewModel _viewModel;
        private BattleAnimationConfig _config;
        private CompositeDisposable _disposables;
        private Color _originalPortraitColor;
        private Vector3 _originalLocalPos;
        private Tween _hitFlashTween;
        private Tween _knockbackTween;


        private void OnDestroy()
        {
            _hitFlashTween?.Kill();
            _knockbackTween?.Kill();
            _disposables?.Dispose();
        }


        public void Bind(HeroSlotViewModel viewModel, IReadyPulseCoordinator pulseCoordinator, BattleAnimationConfig config)
        {
            _viewModel = viewModel;
            _config = config;
            _disposables?.Dispose();
            _disposables = new CompositeDisposable();
            _originalLocalPos = transform.localPosition;

            BindPortrait(viewModel);
            BindHPBars(viewModel);
            BindEnergyBar(viewModel, pulseCoordinator);
            BindHitReaction(viewModel);
        }

        public bool IsValidTarget(UnitDescriptor source)
        {
            if (_viewModel == null || false == _viewModel.IsAssigned || _viewModel.IsDefeated.CurrentValue)
                return false;

            if (source.ActionType == HeroActionType.DealDamage && _viewModel.Side == BattleSide.Enemy)
                return true;

            if (source.ActionType == HeroActionType.HealAlly && _viewModel.Side == BattleSide.Player)
            {
                if (source.Kind == UnitKind.Hero
                    && source.SlotIndex == _viewModel.SlotIndex)
                    return false;

                return _viewModel.HPFill.CurrentValue < 1f;
            }

            return false;
        }

        public void SetSourceHighlight(bool active)
        {
            if (false == _glow)
                return;

            if (active && _config)
                _glow.color = _config.SourceHighlightColor;

            _glow.gameObject.SetActive(active);
        }

        public void SetTargetHighlight(bool active, HeroActionType actionType)
        {
            if (false == _glow)
                return;

            if (active && _config)
                _glow.color = actionType == HeroActionType.HealAlly
                    ? _config.HealTargetColor
                    : _config.AttackTargetColor;

            _glow.gameObject.SetActive(active);
        }


        private void BindPortrait(HeroSlotViewModel viewModel)
        {
            if (false == _portrait)
                return;

            _portrait.enabled = viewModel.IsAssigned;

            if (false == viewModel.IsAssigned)
                return;

            if (_background)
                _background.color = viewModel.SlotColor;

            if (viewModel.Portrait)
                _portrait.sprite = viewModel.Portrait;

            viewModel.IsDefeated
                .Subscribe(defeated =>
                {
                    if (_background)
                        _background.color = defeated ? Color.gray : viewModel.SlotColor;
                })
                .AddTo(_disposables);
        }

        private void BindHPBars(HeroSlotViewModel viewModel)
        {
            if (false == viewModel.IsAssigned)
                return;

            if (_hpBar)
                _hpBar.SetFill(viewModel.HPFill.CurrentValue);

            if (_hpLagBar)
                _hpLagBar.SetFill(viewModel.HPFill.CurrentValue);

            viewModel.SilentDrain
                .Subscribe(fill =>
                {
                    _hpBar?.SetFill(fill);
                    _hpLagBar?.SetFill(fill);
                })
                .AddTo(_disposables);

            viewModel.HPFill
                .Skip(1)
                .Subscribe(fill =>
                {
                    var isDamage = _hpBar != null && fill < _hpBar.CurrentNormalized;

                    if (isDamage)
                    {
                        _hpBar?.SetFill(fill);

                        if (_config)
                            _hpLagBar?.SetFillAnimated(fill, _config.HPBarLagDuration, _config.HPBarLagDelay);
                        else
                            _hpLagBar?.SetFill(fill);
                    }
                    else
                    {
                        var healDuration = _config ? _config.HPBarHealDuration : 0.4f;
                        _hpBar?.SetFillAnimated(fill, healDuration);
                        _hpLagBar?.SetFillAnimated(fill, healDuration);
                    }
                })
                .AddTo(_disposables);

            viewModel.IsDefeated
                .Subscribe(defeated =>
                {
                    if (_hpBar)
                        _hpBar.gameObject.SetActive(false == defeated);

                    if (_hpLagBar)
                        _hpLagBar.gameObject.SetActive(false == defeated);
                })
                .AddTo(_disposables);
        }

        private void BindEnergyBar(HeroSlotViewModel viewModel, IReadyPulseCoordinator pulseCoordinator)
        {
            if (false == _energyBar || false == viewModel.IsAssigned)
                return;

            _energyBar.SetFillColor(viewModel.SlotColor);
            _energyBar.SetFill(viewModel.EnergyFill.CurrentValue);

            viewModel.EnergyFill
                .Skip(1)
                .Subscribe(v =>
                {
                    var duration = _config ? _config.EnergyFillDuration : 0.35f;
                    _energyBar.SetFillAnimated(v, duration);
                })
                .AddTo(_disposables);

            viewModel.IsActivatable
                .Subscribe(activatable =>
                {
                    if (false == activatable)
                        _energyBar.SetFillAlpha(1f);
                })
                .AddTo(_disposables);

            pulseCoordinator.Alpha
                .Subscribe(a =>
                {
                    if (viewModel.IsActivatable.CurrentValue)
                        _energyBar.SetFillAlpha(a);
                })
                .AddTo(_disposables);

            viewModel.IsDefeated
                .Subscribe(defeated => _energyBar.gameObject.SetActive(false == defeated))
                .AddTo(_disposables);
        }

        private void BindHitReaction(HeroSlotViewModel viewModel)
        {
            if (false == viewModel.IsAssigned || false == _portrait || false == _config)
                return;

            _originalPortraitColor = _portrait.color;

            viewModel.Hit
                .Subscribe(_ =>
                {
                    PlayHitFlash();
                    PlayKnockback();
                })
                .AddTo(_disposables);
        }

        private void PlayHitFlash()
        {
            _hitFlashTween?.Kill();
            var halfDuration = _config.HitFlashDuration * 0.5f;

            _hitFlashTween = _portrait
                .DOColor(_config.HitFlashColor, halfDuration)
                .SetEase(_config.HitFlashEase)
                .OnComplete(() =>
                {
                    _hitFlashTween = _portrait
                        .DOColor(_originalPortraitColor, halfDuration)
                        .SetEase(_config.HitFlashEase);
                });
        }

        private void PlayKnockback()
        {
            _knockbackTween?.Kill();
            transform.localPosition = _originalLocalPos;

            var direction = _viewModel.Side == BattleSide.Enemy ? 1f : -1f;
            var targetY = _originalLocalPos.y + direction * _config.KnockbackDistance;
            var halfDuration = _config.KnockbackDuration * 0.5f;

            _knockbackTween = transform
                .DOLocalMoveY(targetY, halfDuration)
                .SetEase(_config.KnockbackEase)
                .OnComplete(() =>
                {
                    _knockbackTween = transform
                        .DOLocalMoveY(_originalLocalPos.y, halfDuration)
                        .SetEase(_config.KnockbackEase);
                });
        }
    }
}