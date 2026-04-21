using Cysharp.Threading.Tasks;
using Project.Scripts.Gameplay.Battle.HUD;
using Project.Scripts.Services.UISystem;
using R3;
using TMPro;
using UnityEngine;

namespace Project.Scripts.Gameplay.UI
{
    public class TopBarView : BaseView<BattleHUDViewModel>
    {
        [Tooltip("Отображает имя противника")]
        [SerializeField] private TMP_Text _enemyNameTMP;

        [Tooltip("Таймер боя")]
        [SerializeField] private TMP_Text _timerTMP;


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