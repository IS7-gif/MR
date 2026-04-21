using DG.Tweening;
using UnityEngine;

namespace Project.Scripts.Gameplay.Battle.Units
{
    public class BarRenderer : MonoBehaviour
    {
        [Tooltip("SpriteRenderer заполнения - прямой дочерний объект этого GameObject")]
        [SerializeField] private SpriteRenderer _fill;

        [Tooltip("При true полоса заполняется снизу вверх вместо слева направо")]
        [SerializeField] private bool _vertical = false;


        public float CurrentNormalized => _currentNormalized;


        private float _currentNormalized;
        private Tween _fillTween;


        private void OnDestroy()
        {
            StopAnimation();
        }


        public void StopAnimation()
        {
            _fillTween?.Kill();
            _fillTween = null;
        }

        public void SnapFill(float normalized)
        {
            StopAnimation();
            SetFill(normalized);
        }

        public void SetFill(float normalized)
        {
            _currentNormalized = Mathf.Clamp01(normalized);

            if (false == _fill)
                return;

            if (_vertical)
            {
                _fill.transform.localScale = new Vector3(1f, _currentNormalized, 1f);
                _fill.transform.localPosition = new Vector3(0f, -(1f - _currentNormalized) * 0.5f, 0f);
            }
            else
            {
                _fill.transform.localScale = new Vector3(_currentNormalized, 1f, 1f);
                _fill.transform.localPosition = new Vector3(-(1f - _currentNormalized) * 0.5f, 0f, 0f);
            }
        }

        public void SetFillAnimated(float normalized, float duration, float delay = 0f)
        {
            _fillTween?.Kill();

            if (delay > 0f)
            {
                var seq = DOTween.Sequence();
                seq.AppendInterval(delay);
                seq.Append(DOTween.To(() => _currentNormalized, SetFill, normalized, duration));
                _fillTween = seq;
            }
            else
                _fillTween = DOTween.To(() => _currentNormalized, SetFill, normalized, duration);
        }

        public void SetFillColor(Color color)
        {
            if (_fill)
                _fill.color = color;
        }

        public void SetFillAlpha(float alpha)
        {
            if (false == _fill)
                return;

            var c = _fill.color;
            c.a = alpha;
            _fill.color = c;
        }
    }
}