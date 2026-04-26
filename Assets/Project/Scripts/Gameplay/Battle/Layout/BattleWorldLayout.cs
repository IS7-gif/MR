using Project.Scripts.Gameplay.Battle.Board;
using Project.Scripts.Gameplay.Battle.HUD;
using UnityEngine;

namespace Project.Scripts.Gameplay.Battle.Layout
{
    public class BattleWorldLayout : MonoBehaviour
    {
        [Tooltip("View матч доски")]
        [SerializeField] private BoardView _boardView;
        
        [Tooltip("Контейнер для тайлов доски")]
        [SerializeField] private Transform _tileContainer;
        
        [Tooltip("View боевого поля")]
        [SerializeField] private BattleFieldView _battleFieldView;
        
        [Tooltip("Вью для отображения энергии игрока и врага в мировом пространстве")]
        [SerializeField] private BattleWorldEnergyView _energyView;


        public BoardView BoardView => _boardView;
        public Transform TileContainer => _tileContainer;
        public BattleFieldView BattleFieldView => _battleFieldView;
        public BattleWorldEnergyView EnergyView => _energyView;


        public void SetBoardWorldCenter(Vector3 boardWorldCenter)
        {
            if (!_boardView)
                return;

            transform.position = boardWorldCenter - _boardView.transform.localPosition;
        }

        public void SetVerticalLayout(float boardTopWorldY, float cellSize,
            float gapBoardToPlayerEnergy, float gapPlayerEnergyToEnemyEnergy, float gapEnemyEnergyToBattleField)
        {
            var cursor = boardTopWorldY + gapBoardToPlayerEnergy * cellSize;

            if (_energyView)
            {
                var playerH = _energyView.PlayerEnergyHeight;
                _energyView.SetPlayerEnergyWorldY(cursor + playerH * 0.5f);
                cursor += playerH + gapPlayerEnergyToEnemyEnergy * cellSize;

                var enemyH = _energyView.EnemyEnergyHeight;
                _energyView.SetEnemyEnergyWorldY(cursor + enemyH * 0.5f);
                cursor += enemyH + gapEnemyEnergyToBattleField * cellSize;
            }

            if (_battleFieldView)
                _battleFieldView.SetLayoutBottomWorldY(cursor);
        }

        public void RefreshBindings()
        {
            _battleFieldView?.RefreshPosition();
        }
    }
}