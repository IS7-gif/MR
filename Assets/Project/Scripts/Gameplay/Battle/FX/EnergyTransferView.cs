using System;
using DG.Tweening;
using Project.Scripts.Configs.Battle;
using UnityEngine;

namespace Project.Scripts.Gameplay.Battle.FX
{
    public class EnergyTransferView : MonoBehaviour
    {
        private const float FullWaveRotation = Mathf.PI * 2f;


        [Tooltip("Sprite renderer used to tint the energy transfer visual")]
        [SerializeField] private SpriteRenderer _sprite;


        private Tween _flightTween;


        private void OnDestroy()
        {
            Kill();
        }


        public void Play(Vector3 from, Vector3 to, Color color, BattleAnimationConfig config, Action onComplete)
        {
            Kill();

            transform.position = from;
            if (_sprite)
                _sprite.color = color;

            var delta = to - from;
            var direction = delta.sqrMagnitude > 0.0001f ? delta.normalized : Vector3.up;
            var side = Vector3.Cross(direction, Vector3.forward);
            if (side.sqrMagnitude <= 0.0001f)
                side = Vector3.right;
            else
                side.Normalize();

            var duration = config ? config.EnergyTransferDuration : 0.45f;
            var amplitude = config ? config.EnergyTransferWaveAmplitude : 0.22f;
            var ease = config ? config.EnergyTransferFlightEase : Ease.InOutSine;
            var progress = 0f;

            _flightTween = DOTween.To(() => progress, x =>
                {
                    progress = x;
                    var basePosition = Vector3.LerpUnclamped(from, to, progress);
                    var waveOffset = Mathf.Sin(progress * FullWaveRotation) * amplitude;
                    transform.position = basePosition + side * waveOffset;
                },
                1f,
                duration)
                .SetEase(ease)
                .OnComplete(() =>
                {
                    _flightTween = null;
                    transform.position = to;
                    onComplete?.Invoke();
                });
        }

        public void Kill()
        {
            _flightTween?.Kill();
            _flightTween = null;
        }
    }
}