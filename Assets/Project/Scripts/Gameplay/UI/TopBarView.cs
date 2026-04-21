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

        [Tooltip("Опциональный TMP для энергии игрока")]
        [SerializeField] private TMP_Text _playerEnergyTMP;

        [Tooltip("Опциональный TMP для энергии противника")]
        [SerializeField] private TMP_Text _enemyEnergyTMP;


        protected override UniTask OnBindViewModel()
        {
            if (_enemyNameTMP)
                _enemyNameTMP.text = ViewModel.EnemyName;

            Disposables.Add(ViewModel.TimerSeconds.Subscribe(OnTimerSecondsChanged));
            Disposables.Add(ViewModel.PlayerEnergy.Subscribe(OnPlayerEnergyChanged));
            Disposables.Add(ViewModel.EnemyEnergy.Subscribe(OnEnemyEnergyChanged));

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

        private void OnPlayerEnergyChanged(int value)
        {
            if (_playerEnergyTMP)
                _playerEnergyTMP.text = value.ToString();
        }

        private void OnEnemyEnergyChanged(int value)
        {
            if (_enemyEnergyTMP)
                _enemyEnergyTMP.text = value.ToString();
        }
    }
}