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
        

        private void OnDestroy()
        {
            StopActiveAnimations();
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

        public UniTask AnimateCollapse()
        {
            _scaleTween?.Kill();
            _scaleTween = transform.DOScale(Vector3.zero, _config.CollapseAllDuration).SetEase(_config.CollapseAllEase);
            
            return AwaitTween(_scaleTween, isMoveTween: false);
        }

        public UniTask AnimateSpawn()
        {
            transform.localScale = Vector3.zero;

            _scaleTween?.Kill();
            _scaleTween = transform.DOScale(_targetScale, _config.SpawnDuration);
            
            return AwaitTween(_scaleTween, isMoveTween: false);
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