using Project.Scripts.Gameplay.Battle.HUD;
using R3;
using TMPro;
using UnityEngine;

namespace Project.Scripts.Gameplay.Battle.Layout
{
    public class BattleWorldEnergyView : MonoBehaviour
    {
        [Tooltip("Текст для отображения текущей энергии врага")]
        [SerializeField] private TMP_Text _enemyEnergyText;
        
        [Tooltip("Текст для отображения текущей энергии игрока")]
        [SerializeField] private TMP_Text _playerEnergyText;


        private CompositeDisposable _disposables;


        private void OnDestroy()
        {
            Cleanup();
        }


        public void Bind(BattleFieldViewModel viewModel)
        {
            Cleanup();
            _disposables = new CompositeDisposable();

            if (viewModel == null)
                return;

            ApplyEnemyEnergy(viewModel.EnemyEnergy.CurrentValue);
            ApplyPlayerEnergy(viewModel.PlayerEnergy.CurrentValue);

            _disposables.Add(viewModel.EnemyEnergy.Subscribe(ApplyEnemyEnergy));
            _disposables.Add(viewModel.PlayerEnergy.Subscribe(ApplyPlayerEnergy));
        }

        public void Cleanup()
        {
            _disposables?.Dispose();
            _disposables = null;
        }


        private void ApplyEnemyEnergy(int value)
        {
            if (_enemyEnergyText)
                _enemyEnergyText.text = value.ToString();
        }

        private void ApplyPlayerEnergy(int value)
        {
            if (_playerEnergyText)
                _playerEnergyText.text = value.ToString();
        }
    }
}