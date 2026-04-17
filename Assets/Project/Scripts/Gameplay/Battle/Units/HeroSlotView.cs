using DG.Tweening;
using Project.Scripts.Configs;
using Project.Scripts.Configs.Battle;
using Project.Scripts.Gameplay.Battle.Targeting;
using Project.Scripts.Shared.Heroes;
using R3;
using TMPro;
using UnityEngine;

namespace Project.Scripts.Gameplay.Battle.Units
{
    public class HeroSlotView : MonoBehaviour, ITargetable
    {
        private static readonly int FillEnabledShaderId = Shader.PropertyToID("_FillEnabled");
        private static readonly int FillReplaceShaderId = Shader.PropertyToID("_FillReplace");

        [Tooltip("SpriteRenderer, определяющий границы слота для таргетинга; не используется для окраски")]
        [SerializeField] private SpriteRenderer _boundsSource;

        [Tooltip("SpriteRenderers, которые красятся цветом элемента героя; при смерти красятся в цвет смерти из UnitDeathConfig")]
        [SerializeField] private SpriteRenderer[] _coloredRenderers;

        [Tooltip("Portrait SpriteRenderer - displays the character sprite and flashes on hit")]
        [SerializeField] private SpriteRenderer _portrait;

        [Tooltip("SpriteRenderer свечения с материалом Additive - отображается как подсветка источника или цели")]
        [SerializeField] private SpriteRenderer _glow;

        [Tooltip("Корневой объект секции HP (содержит фон, HPBar и HPLagBar) - скрывается целиком при смерти")]
        [SerializeField] private GameObject _hpBarContainer;

        [Tooltip("Основная полоса HP - мгновенно обновляется при получении урона")]
        [SerializeField] private BarRenderer _hpBar;

        [Tooltip("Лаг-полоса HP позади основной полосы - опустошается с задержкой после получения урона")]
        [SerializeField] private BarRenderer _hpLagBar;

        [Tooltip("Полоса энергии")]
        [SerializeField] private BarRenderer _energyBar;

        [Tooltip("Текст HP в формате 'Current / Max' - скрывается при MaxHP = 0 (бессмертный юнит)")]
        [SerializeField] private TMP_Text _hpText;

        [Tooltip("Якорь для всплывающих чисел урона/лечения - по умолчанию центр слота, если не назначен")]
        [SerializeField] private Transform _hitAnchor;

        public Transform HitAnchor => _hitAnchor ? _hitAnchor : transform;
        public UnitDescriptor Descriptor => UnitDescriptor.Hero(_viewModel.Side, _viewModel.SlotIndex, _viewModel.ActionType);
        public bool IsReadySource => _viewModel != null && _viewModel.IsAssigned && _viewModel.IsActivatable.CurrentValue;
        public Bounds WorldBounds => _boundsSource ? _boundsSource.bounds : new Bounds(transform.position, Vector3.one);

        private HeroSlotViewModel _viewModel;
        private BattleAnimationConfig _config;
        private UnitDeathConfig _deathConfig;
        private CompositeDisposable _disposables;
        private Color _originalPortraitColor;
        private Vector3 _originalLocalPos;
        private Tween _hitFlashTween;
        private Tween _knockbackTween;
        private MaterialPropertyBlock _portraitPropertyBlock;

        private void OnDestroy()
        {
            _hitFlashTween?.Kill();
            _knockbackTween?.Kill();
            _disposables?.Dispose();
        }

        public void Bind(HeroSlotViewModel viewModel, IReadyPulseCoordinator pulseCoordinator, BattleAnimationConfig config, UnitDeathConfig deathConfig)
        {
            _viewModel = viewModel;
            _config = config;
            _deathConfig = deathConfig;

            _disposables?.Dispose();
            _disposables = new CompositeDisposable();
            _originalLocalPos = transform.localPosition;
            ResetPortraitDeathFill();

            BindPortrait(viewModel);
            BindHPBars(viewModel);
            BindEnergyBar(viewModel, pulseCoordinator);
            BindHitReaction(viewModel);
            BindDeathState(viewModel);
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

            ResetPortraitDeathFill();
            _portrait.enabled = viewModel.IsAssigned;
            _originalPortraitColor = _portrait.color;

            if (false == viewModel.IsAssigned)
                return;

            ApplySlotColor(viewModel.SlotColor);

            if (viewModel.Portrait)
                _portrait.sprite = viewModel.Portrait;
        }

        private void BindHPBars(HeroSlotViewModel viewModel)
        {
            if (false == viewModel.IsAssigned)
                return;

            if (_hpBar)
                _hpBar.SnapFill(viewModel.HPFill.CurrentValue);

            if (_hpLagBar)
                _hpLagBar.SnapFill(viewModel.HPFill.CurrentValue);

            if (_hpText)
                _hpText.gameObject.SetActive(true);

            SetHPText(viewModel.CurrentHP, viewModel.MaxHP);

            viewModel.HealthBarUpdated
                .Subscribe(ApplyHealthBarUpdate)
                .AddTo(_disposables);
        }

        private void BindEnergyBar(HeroSlotViewModel viewModel, IReadyPulseCoordinator pulseCoordinator)
        {
            if (false == _energyBar || false == viewModel.IsAssigned)
                return;

            _energyBar.SnapFill(viewModel.EnergyFill.CurrentValue);

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

        private void BindDeathState(HeroSlotViewModel viewModel)
        {
            viewModel.IsDefeated
                .Subscribe(defeated =>
                {
                    var visuals = _deathConfig ? _deathConfig.HeroDeathVisuals : default;

                    if (defeated)
                        FinalizeDeathState(viewModel.HPFill.CurrentValue);

                    if (visuals.ChangeBackgroundColor)
                        ApplySlotColor(defeated ? visuals.DeathBackgroundColor : viewModel.SlotColor);

                    if (visuals.ApplyDeathFill)
                        SetPortraitDeathFill(defeated);

                    if (_portrait && visuals.DisablePortrait)
                        _portrait.enabled = false == defeated;

                    if (visuals.DisableHpBar && _hpBarContainer)
                        _hpBarContainer.SetActive(false == defeated);

                    if (_hpText)
                        _hpText.gameObject.SetActive(false == defeated);

                    if (_energyBar && visuals.DisableEnergyBar)
                        _energyBar.gameObject.SetActive(false == defeated);
                })
                .AddTo(_disposables);
        }

        private void ApplyHealthBarUpdate(HealthBarUpdate update)
        {
            SetHPText(update.CurrentHP, update.MaxHP);

            if (update.Mode == HealthBarUpdateMode.Snap)
            {
                _hpBar?.SnapFill(update.Fill);
                _hpLagBar?.SnapFill(update.Fill);
                return;
            }

            if (update.Mode == HealthBarUpdateMode.Damage)
            {
                _hpBar?.SetFill(update.Fill);

                if (_config)
                    _hpLagBar?.SetFillAnimated(update.Fill, _config.HPBarLagDuration, _config.HPBarLagDelay);
                else
                    _hpLagBar?.SetFill(update.Fill);

                return;
            }

            var healDuration = _config ? _config.HPBarHealDuration : 0.4f;
            _hpBar?.SetFillAnimated(update.Fill, healDuration);
            _hpLagBar?.SetFillAnimated(update.Fill, healDuration);
        }

        private void SetHPText(int currentHP, int maxHP)
        {
            if (false == _hpText)
                return;

            _hpText.text = maxHP > 0 ? $"{currentHP} / {maxHP}" : string.Empty;
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

        private void FinalizeDeathState(float hpFill)
        {
            _hitFlashTween?.Kill();
            _hitFlashTween = null;
            _knockbackTween?.Kill();
            _knockbackTween = null;
            transform.localPosition = _originalLocalPos;

            if (_portrait)
                _portrait.color = _originalPortraitColor;

            _hpBar?.SnapFill(hpFill);
            _hpLagBar?.SnapFill(hpFill);

            if (_energyBar)
                _energyBar.SnapFill(_viewModel.EnergyFill.CurrentValue);
        }

        private void SetPortraitDeathFill(bool active)
        {
            if (false == _portrait)
                return;

            _portraitPropertyBlock ??= new MaterialPropertyBlock();

            if (false == active)
            {
                _portraitPropertyBlock.Clear();
                _portrait.SetPropertyBlock(_portraitPropertyBlock);
                return;
            }

            _portraitPropertyBlock.Clear();
            _portraitPropertyBlock.SetFloat(FillEnabledShaderId, 1f);
            _portraitPropertyBlock.SetFloat(FillReplaceShaderId, 1f);
            _portrait.SetPropertyBlock(_portraitPropertyBlock);
        }

        private void ResetPortraitDeathFill()
        {
            SetPortraitDeathFill(false);
        }

        private void ApplySlotColor(Color color)
        {
            if (_coloredRenderers == null)
                return;

            for (var i = 0; i < _coloredRenderers.Length; i++)
                if (_coloredRenderers[i])
                    _coloredRenderers[i].color = color;
        }
    }
}