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

        [Tooltip("Маркер высоты, на которой сферы энергии игрока поглощаются общим запасом")]
        [SerializeField] private Transform _playerEnergyAbsorbTarget;


        private CompositeDisposable _disposables;


        public Transform PlayerEnergyAbsorbTarget => _playerEnergyAbsorbTarget ? _playerEnergyAbsorbTarget : _playerEnergyText ? _playerEnergyText.transform : null;
        public float PlayerEnergyHeight => _playerEnergyText ? _playerEnergyText.preferredHeight : 0f;
        public float EnemyEnergyHeight => _enemyEnergyText ? _enemyEnergyText.preferredHeight : 0f;


        private void OnDestroy()
        {
            Cleanup();
        }


        public void SetPlayerEnergyWorldY(float worldCenterY)
        {
            if (false == _playerEnergyText)
                return;

            var textPos = _playerEnergyText.transform.position;
            _playerEnergyText.transform.position = new Vector3(textPos.x, worldCenterY, textPos.z);

            if (_playerEnergyAbsorbTarget && _playerEnergyAbsorbTarget.parent != _playerEnergyText.transform)
            {
                var absorbPos = _playerEnergyAbsorbTarget.position;
                _playerEnergyAbsorbTarget.position = new Vector3(absorbPos.x, worldCenterY, absorbPos.z);
            }
        }

        public void SetEnemyEnergyWorldY(float worldCenterY)
        {
            if (false == _enemyEnergyText)
                return;

            var pos = _enemyEnergyText.transform.position;
            _enemyEnergyText.transform.position = new Vector3(pos.x, worldCenterY, pos.z);
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