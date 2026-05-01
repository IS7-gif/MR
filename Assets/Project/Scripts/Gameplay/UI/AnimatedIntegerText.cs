using System;
using DG.Tweening;
using TMPro;

namespace Project.Scripts.Gameplay.UI
{
    public sealed class AnimatedIntegerText : IDisposable
    {
        private readonly TMP_Text _label;
        private Tween _tween;
        private int _displayedValue;
        private bool _hasValue;


        public AnimatedIntegerText(TMP_Text label)
        {
            _label = label;
        }

        public void SetInstant(int value)
        {
            _tween?.Kill();
            _tween = null;
            _displayedValue = value;
            _hasValue = true;
            SetText(value);
        }

        public void AnimateTo(int value, float duration, Ease ease)
        {
            if (false == _label)
                return;

            if (false == _hasValue || duration <= 0f)
            {
                SetInstant(value);
                return;
            }

            if (_displayedValue == value)
                return;

            _tween?.Kill();
            _tween = DOTween.To(
                    () => _displayedValue,
                    SetAnimatedValue,
                    value,
                    duration)
                .SetEase(ease)
                .SetLink(_label.gameObject);
        }

        public void Dispose()
        {
            _tween?.Kill();
            _tween = null;
        }

        private void SetAnimatedValue(int value)
        {
            _displayedValue = value;
            SetText(value);
        }

        private void SetText(int value)
        {
            if (_label)
                _label.text = value.ToString();
        }
    }
}
