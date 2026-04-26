using Cysharp.Threading.Tasks;
using Project.Scripts.Gameplay.Battle.HUD;
using Project.Scripts.Services.UISystem;
using R3;
using TMPro;
using UnityEngine;

namespace Project.Scripts.Gameplay.UI
{
    public class TopBarView : BaseView<BattleFieldViewModel>
    {
        [Tooltip("Отображает имя противника")]
        [SerializeField] private TMP_Text _enemyNameTMP;

        [Tooltip("Таймер боя")]
        [SerializeField] private TMP_Text _timerTMP;


        public void ApplyScreenRect(Rect screenRect)
        {
            if (transform is not RectTransform rectTransform)
                return;

            if (Screen.width <= 0 || Screen.height <= 0)
                return;

            rectTransform.anchorMin = new Vector2(
                screenRect.xMin / Screen.width,
                screenRect.yMin / Screen.height);
            rectTransform.anchorMax = new Vector2(
                screenRect.xMax / Screen.width,
                screenRect.yMax / Screen.height);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }


        protected override UniTask OnBindViewModel()
        {
            if (_enemyNameTMP)
                _enemyNameTMP.text = ViewModel.EnemyName;

            Disposables.Add(ViewModel.TimerSeconds.Subscribe(OnTimerSecondsChanged));

            return UniTask.CompletedTask;
        }


        private void OnTimerSecondsChanged(int totalSeconds)
        {
            if (false == _timerTMP)
                return;

            var minutes = totalSeconds / 60;
            var seconds = totalSeconds % 60;
            _timerTMP.text = $"{minutes:00}:{seconds:00}";
        }
    }
}