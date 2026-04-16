using DG.Tweening;
using Project.Scripts.Configs;
using Project.Scripts.Configs.Battle;
using Project.Scripts.Gameplay.Battle.Targeting;
using Project.Scripts.Gameplay.UI;
using Project.Scripts.Services.Combat;
using Project.Scripts.Shared.Heroes;
using R3;
using UnityEngine;

namespace Project.Scripts.Gameplay.Battle.Units
{
    public class AvatarSlotView : MonoBehaviour, ITargetable
    {
        private static readonly int FillEnabledShaderId = Shader.PropertyToID("_FillEnabled");
        private static readonly int FillReplaceShaderId = Shader.PropertyToID("_FillReplace");

        [Tooltip("Фоновый SpriteRenderer - определяет границы слота и масштабируется через SetSize")]
        [SerializeField] private SpriteRenderer _background;

        [Tooltip("SpriteRenderer портрета - отображает изображение аватара")]
        [SerializeField] private SpriteRenderer _portrait;

        [Tooltip("SpriteRenderer свечения с материалом Additive - отображается как подсветка источника или цели")]
        [SerializeField] private SpriteRenderer _glow;

        [Tooltip("Основная полоса HP - мгновенно обновляется при получении урона")]
        [SerializeField] private BarRenderer _hpBar;

        [Tooltip("Лаг-полоса HP позади основной полосы - опустошается с задержкой после получения урона")]
        [SerializeField] private BarRenderer _hpLagBar;

        [Tooltip("Полоса энергии/заряда - заполняется по мере накопления энергии аватаром")]
        [SerializeField] private BarRenderer _energyBar;

        [Tooltip("Transform, используемый как якорь для всплывающих чисел урона/лечения")]
        [SerializeField] private Transform _hitAnchor;


        public UnitDescriptor Descriptor => UnitDescriptor.Avatar(_viewModel.Side, _viewModel.AbilityType);
        public bool IsReadySource => _viewModel != null && _viewModel.Side == BattleSide.Player && _viewModel.EnergyBar.IsReady.CurrentValue;
        public Bounds WorldBounds => _background ? _background.bounds : new Bounds(transform.position, Vector3.one);
        public Transform HitAnchor => _hitAnchor ? _hitAnchor : transform;


        private AvatarSlotViewModel _viewModel;
        private IAvatarGroupDefenseService _groupDefense;
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


        public void Bind(AvatarSlotViewModel viewModel, IReadyPulseCoordinator pulseCoordinator, IAvatarGroupDefenseService groupDefense, UnitDeathConfig deathConfig)
        {
            _viewModel = viewModel;
            _groupDefense = groupDefense;
            _config = viewModel.AnimConfig;
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
            if (null == _viewModel || _viewModel.IsDefeated.CurrentValue)
                return false;

            if (source.ActionType == HeroActionType.DealDamage && _viewModel.Side == BattleSide.Enemy)
            {
                if (_groupDefense != null && false == _groupDefense.IsExposed(BattleSide.Enemy))
                    return false;

                return true;
            }

            if (source.ActionType == HeroActionType.HealAlly && _viewModel.Side == BattleSide.Player)
            {
                if (source.Kind == UnitKind.Avatar)
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


        private void BindPortrait(AvatarSlotViewModel viewModel)
        {
            if (_background)
                _background.color = viewModel.SlotColor;

            if (false == _portrait)
                return;

            ResetPortraitDeathFill();
            _portrait.sprite = viewModel.Portrait;
            _originalPortraitColor = _portrait.color;
        }

        private void BindHPBars(AvatarSlotViewModel viewModel)
        {
            if (_hpBar)
                _hpBar.SnapFill(viewModel.HPFill.CurrentValue);

            if (_hpLagBar)
                _hpLagBar.SnapFill(viewModel.HPFill.CurrentValue);

            viewModel.HealthBarUpdated
                .Subscribe(ApplyHealthBarUpdate)
                .AddTo(_disposables);
        }

        private void BindEnergyBar(AvatarSlotViewModel viewModel, IReadyPulseCoordinator pulseCoordinator)
        {
            if (false == _energyBar)
                return;

            _energyBar.SnapFill(viewModel.EnergyBar.FillFraction.CurrentValue);

            viewModel.EnergyBar.FillFraction
                .Skip(1)
                .Subscribe(v =>
                {
                    var duration = _config ? _config.EnergyFillDuration : 0.35f;
                    _energyBar.SetFillAnimated(v, duration);
                })
                .AddTo(_disposables);

            viewModel.EnergyBar.IsReady
                .Subscribe(ready =>
                {
                    if (false == ready)
                        _energyBar.SetFillAlpha(1f);
                })
                .AddTo(_disposables);

            pulseCoordinator.Alpha
                .Subscribe(a =>
                {
                    if (viewModel.EnergyBar.IsReady.CurrentValue)
                        _energyBar.SetFillAlpha(a);
                })
                .AddTo(_disposables);
        }

        private void BindHitReaction(AvatarSlotViewModel viewModel)
        {
            if (false == _portrait || false == _config)
                return;

            viewModel.Hit
                .Subscribe(_ =>
                {
                    PlayHitFlash();
                    PlayKnockback();
                })
                .AddTo(_disposables);
        }

        private void BindDeathState(AvatarSlotViewModel viewModel)
        {
            viewModel.IsDefeated
                .Subscribe(defeated =>
                {
                    var visuals = _deathConfig ? _deathConfig.AvatarDeathVisuals : default;

                    if (defeated)
                        FinalizeDeathState(viewModel.HPFill.CurrentValue);

                    if (_background && visuals.ChangeBackgroundColor)
                        _background.color = defeated ? visuals.DeathBackgroundColor : viewModel.SlotColor;

                    if (visuals.ApplyDeathFill)
                        SetPortraitDeathFill(defeated);

                    if (_portrait && visuals.DisablePortrait)
                        _portrait.enabled = false == defeated;

                    if (visuals.DisableHpBar)
                    {
                        if (_hpBar)
                            _hpBar.gameObject.SetActive(false == defeated);

                        if (_hpLagBar)
                            _hpLagBar.gameObject.SetActive(false == defeated);
                    }

                    if (_energyBar && visuals.DisableEnergyBar)
                        _energyBar.gameObject.SetActive(false == defeated);
                })
                .AddTo(_disposables);
        }

        private void ApplyHealthBarUpdate(HealthBarUpdate update)
        {
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
                _energyBar.SnapFill(_viewModel.EnergyBar.FillFraction.CurrentValue);
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
