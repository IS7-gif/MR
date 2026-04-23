using UnityEngine;

namespace Project.Scripts.Gameplay.Battle.Units
{
    public class CooldownSweepView : MonoBehaviour
    {
        private static readonly int ProgressShaderId = Shader.PropertyToID("_Progress");

        
        [Tooltip("SpriteRenderer for the radial cooldown overlay - same sprite as portrait, sorting order above it")]
        [SerializeField] private SpriteRenderer _overlay;


        private MaterialPropertyBlock _propertyBlock;


        public void SetSprite(Sprite sprite)
        {
            if (!_overlay)
                return;

            _overlay.sprite = sprite;
        }

        public void SetCooldown(float remaining, float duration)
        {
            if (!_overlay)
                return;

            var progress = duration > 0f ? Mathf.Clamp01(remaining / duration) : 0f;
            _overlay.enabled = progress > 0f;

            if (progress <= 0f)
                return;

            _propertyBlock ??= new MaterialPropertyBlock();
            _propertyBlock.SetFloat(ProgressShaderId, progress);
            _overlay.SetPropertyBlock(_propertyBlock);
        }
    }
}