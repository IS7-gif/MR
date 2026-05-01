using Project.Scripts.Gameplay.Battle.HUD;
using Project.Scripts.Services.Announcements;
using R3;
using UnityEngine;

namespace Project.Scripts.Gameplay.Battle.Layout
{
    public class BattleWorldEnergyView : MonoBehaviour
    {
        [Tooltip("Бар энергии игрока")]
        [SerializeField] private EnergyBarView _playerBar;

        [Tooltip("Бар энергии врага")]
        [SerializeField] private EnergyBarView _enemyBar;

        [Tooltip("Маркер высоты, на которой сферы энергии игрока поглощаются общим запасом")]
        [SerializeField] private Transform _playerEnergyAbsorbTarget;


        public Transform PlayerEnergyAbsorbTarget => _playerEnergyAbsorbTarget ? _playerEnergyAbsorbTarget : _playerBar ? _playerBar.transform : null;
        public float PlayerEnergyHeight => _playerBar ? _playerBar.Height : 0f;
        public float EnemyEnergyHeight => _enemyBar ? _enemyBar.Height : 0f;
        public float PlayerEnergyBaseHeight => _playerBar ? _playerBar.BaseHeight : 0f;
        public float EnemyEnergyBaseHeight => _enemyBar ? _enemyBar.BaseHeight : 0f;


        private CompositeDisposable _disposables;


        private void OnDestroy()
        {
            Cleanup();
        }


        public void SetPlayerEnergyWorldY(float worldCenterY)
        {
            _playerBar?.SetWorldCenterY(worldCenterY);

            if (_playerEnergyAbsorbTarget && _playerBar && _playerEnergyAbsorbTarget.parent != _playerBar.transform)
            {
                var absorbPos = _playerEnergyAbsorbTarget.position;
                _playerEnergyAbsorbTarget.position = new Vector3(absorbPos.x, worldCenterY, absorbPos.z);
            }
        }

        public void SetEnemyEnergyWorldY(float worldCenterY)
        {
            _enemyBar?.SetWorldCenterY(worldCenterY);
        }

        public void SetLayoutScale(float scale)
        {
            _playerBar?.SetLayoutScale(scale);
            _enemyBar?.SetLayoutScale(scale);
        }

        public void Bind(BattleFieldViewModel viewModel, IBoardAnnouncementService announcementService = null)
        {
            Cleanup();
            _disposables = new CompositeDisposable();

            if (viewModel == null)
                return;

            _playerBar?.SetValue(viewModel.PlayerEnergy.CurrentValue, false);
            _enemyBar?.SetValue(viewModel.EnemyEnergy.CurrentValue, false);

            _disposables.Add(viewModel.PlayerEnergy.Subscribe(v => _playerBar?.SetValue(v)));
            _disposables.Add(viewModel.EnemyEnergy.Subscribe(v => _enemyBar?.SetValue(v)));

            if (announcementService != null)
            {
                _disposables.Add(announcementService.IsEnergyTextHidden.Subscribe(hidden =>
                {
                    _playerBar?.SetTextVisible(!hidden);
                    _enemyBar?.SetTextVisible(!hidden);
                }));
            }
        }

        public void Cleanup()
        {
            _disposables?.Dispose();
            _disposables = null;
        }
    }
}
