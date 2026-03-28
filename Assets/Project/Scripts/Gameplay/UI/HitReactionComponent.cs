using DG.Tweening;
using Project.Scripts.Configs;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Gameplay.UI
{
    public class HitReactionComponent : MonoBehaviour
    {
        [Tooltip("Portrait Image of this unit — flashes to hit color on damage")]
        [SerializeField] private Image _portrait;

        [Tooltip("RectTransform to move during knockback — assign the avatar+HP sub-container, not the full panel. If left empty, uses this GameObject's own RectTransform")]
        [SerializeField] private RectTransform _knockbackTarget;

        [Tooltip("Knockback direction along Y axis: +1 = up (enemy side), -1 = down (player side)")]
        [SerializeField] private float _knockbackDirectionSign = 1f;


        private BattleAnimationConfig _config;
        private Color _portraitBaseColor;


        public void Init(BattleAnimationConfig config)
        {
            _config = config;
            _portraitBaseColor = _portrait ? _portrait.color : Color.white;
        }

        public void PlayHitReaction()
        {
            if (!_config)
                return;

            PlayHitFlash();
            PlayHitKnockback();
        }


        private void PlayHitFlash()
        {
            if (!_portrait)
                return;

            _portrait.DOKill();
            var halfDuration = _config.HitFlashDuration * 0.5f;
            DOTween.Sequence()
                .Append(_portrait.DOColor(_config.HitFlashColor, halfDuration).SetEase(_config.HitFlashEase))
                .Append(_portrait.DOColor(_portraitBaseColor, halfDuration).SetEase(_config.HitFlashEase));
        }

        private void PlayHitKnockback()
        {
            var rect = _knockbackTarget ? _knockbackTarget : transform as RectTransform;
            if (!rect)
                return;

            rect.DOKill();
            var startY = rect.anchoredPosition.y;
            var halfDuration = _config.KnockbackDuration * 0.5f;
            DOTween.Sequence()
                .Append(rect.DOAnchorPosY(startY + _knockbackDirectionSign * _config.KnockbackDistance, halfDuration).SetEase(_config.KnockbackEase))
                .Append(rect.DOAnchorPosY(startY, halfDuration).SetEase(_config.KnockbackEase));
        }
    }
}