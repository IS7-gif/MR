using DG.Tweening;
using UnityEngine;

namespace Project.Scripts.Configs.Board
{
    [CreateAssetMenu(fileName = "BoardAnimationConfig", menuName = "Configs/Board Animation Config")]
    public class BoardAnimationConfig : ScriptableObject
    {
        [Tooltip("Длительность анимации обмена тайлов в секундах")]
        [SerializeField] private float _swapDuration = 0.2f;

        [Tooltip("Длительность анимации падения тайлов в секундах")]
        [SerializeField] private float _fallDuration = 0.15f;

        [Tooltip("Кривая ослабления для падающих тайлов")]
        [SerializeField] private Ease _fallEase = Ease.OutBounce;

        [Tooltip("Длительность анимации уничтожения тайлов в секундах")]
        [SerializeField] private float _destroyDuration = 0.1f;

        [Tooltip("Кривая ослабления для уничтожения тайлов")]
        [SerializeField] private Ease _destroyEase = Ease.InBack;

        [Tooltip("Длительность анимации появления тайлов в секундах")]
        [SerializeField] private float _spawnDuration = 0.15f;

        [Tooltip("Пауза в секундах между последовательными волнами цепной реакции (бомба - линейная руна - следующий взрыв и т.д.)")]
        [SerializeField] private float _chainReactionWaveDelay = 0.12f;


        public float SwapDuration => _swapDuration;
        public float FallDuration => _fallDuration;
        public Ease FallEase => _fallEase;
        public float DestroyDuration => _destroyDuration;
        public Ease DestroyEase => _destroyEase;
        public float SpawnDuration => _spawnDuration;
        public float ChainReactionWaveDelay => _chainReactionWaveDelay;
    }
}