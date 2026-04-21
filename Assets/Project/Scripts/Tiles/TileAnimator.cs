using Cysharp.Threading.Tasks;
using DG.Tweening;
using Project.Scripts.Configs.Board;
using UnityEngine;

namespace Project.Scripts.Tiles
{
    public class TileAnimator : MonoBehaviour
    {
        private BoardAnimationConfig _config;
        private Vector3 _targetScale = Vector3.one;
        private Tween _moveTween;
        private Tween _scaleTween;
        private Sequence _hintSequence;
        

        private void OnDestroy()
        {
            StopActiveAnimations();
            StopHintPulse();
        }
        

        public void Init(BoardAnimationConfig config)
        {
            _config = config;
        }

        public void SetTargetScale(float cellSize)
        {
            _targetScale = Vector3.one * cellSize;
        }

        public void StopActiveAnimations()
        {
            _moveTween?.Kill();
            _moveTween = null;

            _scaleTween?.Kill();
            _scaleTween = null;
        }

        public UniTask AnimateSwapTo(Vector3 target)
        {
            _moveTween?.Kill();
            _moveTween = transform.DOMove(target, _config.SwapDuration);
            
            return AwaitTween(_moveTween, isMoveTween: true);
        }

        public UniTask AnimateFallTo(Vector3 target)
        {
            _moveTween?.Kill();
            _moveTween = transform.DOMove(target, _config.FallDuration).SetEase(_config.FallEase);
            
            return AwaitTween(_moveTween, isMoveTween: true);
        }

        public UniTask AnimateDestroy()
        {
            _scaleTween?.Kill();
            _scaleTween = transform.DOScale(Vector3.zero, _config.DestroyDuration).SetEase(_config.DestroyEase);
           
            return AwaitTween(_scaleTween, isMoveTween: false);
        }

        public UniTask AnimateCollapse(float duration, Ease ease)
        {
            _scaleTween?.Kill();
            _scaleTween = transform.DOScale(Vector3.zero, duration).SetEase(ease);
            
            return AwaitTween(_scaleTween, isMoveTween: false);
        }

        public UniTask AnimateSpawn()
        {
            transform.localScale = Vector3.zero;

            _scaleTween?.Kill();
            _scaleTween = transform.DOScale(_targetScale, _config.SpawnDuration);

            return AwaitTween(_scaleTween, isMoveTween: false);
        }

        public void AnimateHintPulse(HintConfig config)
        {
            StopHintPulse();

            var pulseScale = _targetScale * config.PulseScaleMax;
            var halfDuration = config.PulseDuration * 0.5f;

            _hintSequence = DOTween.Sequence();
            _hintSequence.Append(transform.DOScale(pulseScale, halfDuration).SetEase(Ease.InOutSine));
            _hintSequence.Append(transform.DOScale(_targetScale, halfDuration).SetEase(Ease.InOutSine));
            _hintSequence.AppendInterval(config.PauseBetweenPulses);
            _hintSequence.SetLoops(-1);
        }

        public void StopHintPulse()
        {
            if (null == _hintSequence)
                return;

            _hintSequence.Kill();
            _hintSequence = null;
            transform.localScale = _targetScale;
        }


        private UniTask AwaitTween(Tween tween, bool isMoveTween)
        {
            var tcs = new UniTaskCompletionSource();
            tween.OnComplete(() => tcs.TrySetResult());
            tween.OnKill(() =>
            {
                if (isMoveTween)
                {
                    if (_moveTween == tween)
                        _moveTween = null;
                }
                else
                {
                    if (_scaleTween == tween)
                        _scaleTween = null;
                }

                tcs.TrySetResult();
            });

            return tcs.Task;
        }
    }
}