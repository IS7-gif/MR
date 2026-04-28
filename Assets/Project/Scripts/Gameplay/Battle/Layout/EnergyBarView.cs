using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Project.Scripts.Gameplay.Battle.Layout
{
    public class EnergyBarView : MonoBehaviour
    {
        private const int VisualMax = 250;


        [Tooltip("SpriteRenderer фона бара (полная ширина)")]
        [SerializeField] private SpriteRenderer _background;

        [Tooltip("SpriteRenderer заполнения бара (масштабируется пропорционально энергии)")]
        [SerializeField] private SpriteRenderer _fill;

        [Tooltip("Текстовая метка с текущим значением энергии")]
        [SerializeField] private TMP_Text _valueText;

        [Tooltip("Ширина бара в мировых единицах")]
        [SerializeField] private float _barWidth = 2f;

        [Tooltip("Высота бара в мировых единицах")]
        [SerializeField] private float _barHeight = 0.35f;

        [Tooltip("Длительность анимации заполнения в секундах")]
        [SerializeField] private float _animDuration = 0.3f;

        [Tooltip("Кривая плавности анимации заполнения")]
        [SerializeField] private Ease _animEase = Ease.OutQuad;


        public float Height => _barHeight;


        private float _displayedRatio;
        private Tween _fillTween;


        private void OnDestroy()
        {
            _fillTween?.Kill();
        }

        private void Awake()
        {
            ApplyBackground();
            ApplyFill(0f);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            ApplyBackground();
            ApplyFill(_displayedRatio);
        }
#endif


        public void SetValue(int value)
        {
            if (_valueText)
                _valueText.text = value.ToString();

            var targetRatio = Mathf.Clamp01(value / (float)VisualMax);
            _fillTween?.Kill();
            _fillTween = DOTween.To(
                    () => _displayedRatio,
                    r => { _displayedRatio = r; ApplyFill(r); },
                    targetRatio,
                    _animDuration)
                .SetEase(_animEase);
        }

        public void SetWorldCenterY(float worldY)
        {
            var pos = transform.position;
            transform.position = new Vector3(pos.x, worldY, pos.z);
        }

        public void SetTextVisible(bool visible)
        {
            if (_valueText)
                _valueText.enabled = visible;
        }


        private void ApplyBackground()
        {
            if (false == _background || false == _background.sprite)
                return;

            ApplySpriteSize(_background, _barWidth, _barHeight);
        }

        private void ApplyFill(float ratio)
        {
            if (false == _fill || false == _fill.sprite)
                return;

            var fillWidth = _barWidth * ratio;
            ApplySpriteSize(_fill, fillWidth, _barHeight);

            var local = _fill.transform.localPosition;
            _fill.transform.localPosition = new Vector3(_barWidth * (ratio - 1f) * 0.5f, local.y, local.z);
        }

        private static void ApplySpriteSize(SpriteRenderer sr, float worldWidth, float worldHeight)
        {
            var spriteSize = sr.sprite.bounds.size;
            var parentScale = sr.transform.parent ? sr.transform.parent.lossyScale : Vector3.one;
            sr.transform.localScale = new Vector3(
                spriteSize.x > 0f ? worldWidth / (spriteSize.x * parentScale.x) : 1f,
                spriteSize.y > 0f ? worldHeight / (spriteSize.y * parentScale.y) : 1f,
                1f);
        }
    }
}