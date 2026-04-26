using Project.Scripts.Configs.Battle;
using Project.Scripts.Configs.Board;
using Project.Scripts.Configs.Grid;
using Project.Scripts.Gameplay.Battle.Layout;
using UnityEngine;

namespace Project.Scripts.Gameplay.Battle.Board
{
    [ExecuteAlways]
    public class BoardPositioner : MonoBehaviour
    {
        [SerializeField] private BoardConfig _boardConfig;
        [SerializeField] private BattleViewConfig _battleViewConfig;
        [SerializeField] private GridConfig _gridConfig;


        private void Update()
        {
            if (Application.isPlaying)
                return;

            Apply();
        }

        public void Apply(float tileCellSize = -1f)
        {
            var cam = Camera.main;
            if (!cam || !_boardConfig || !_battleViewConfig || !_gridConfig)
                return;

            var (frameWidth, frameHeight, frameCellSize) = ComputeFrameDimensions(cam);

            if (tileCellSize < 0f)
                tileCellSize = ComputeTileCellSize(cam);

            var boardView = GetComponent<BoardView>();
            if (boardView)
                boardView.Setup(frameWidth, frameHeight, tileCellSize, _boardConfig.MaskTopPadding);

            var boardCenter = ComputeBoardCenter(cam, frameHeight, frameCellSize);
            var layout = GetComponentInParent<BattleWorldLayout>();
            if (layout)
            {
                layout.SetBoardWorldCenter(boardCenter);

                var boardTopWorldY = boardCenter.y + frameHeight * 0.5f;
                layout.SetVerticalLayout(
                    boardTopWorldY,
                    tileCellSize,
                    _battleViewConfig.GapBoardToPlayerEnergy,
                    _battleViewConfig.GapPlayerEnergyToEnemyEnergy,
                    _battleViewConfig.GapEnemyEnergyToBattleField);
                layout.RefreshBindings();
                return;
            }

            transform.position = boardCenter;
        }


        private Vector3 ComputeBoardCenter(Camera cam, float frameHeight, float frameCellSize)
        {
            var camBottomY = cam.transform.position.y - cam.orthographicSize;
            var bottomPadding = _battleViewConfig.BattleWorldBottomPadding * frameCellSize;

            return new Vector3(cam.transform.position.x, camBottomY + bottomPadding + frameHeight * 0.5f, 0f);
        }

        private (float width, float height, float cellSize) ComputeFrameDimensions(Camera cam)
        {
            var camHeight = cam.orthographicSize * 2f;
            var camWidth = camHeight * GetAspect(cam);
            var effectiveWidth = Mathf.Min(camWidth, camHeight * _boardConfig.MaxAspectRatio);

            var byWidth = effectiveWidth * (1f - _boardConfig.FramePaddingPercent) / _gridConfig.Width;
            var byHeight = camHeight * (1f - _boardConfig.UIReservedHeightPercent) / _gridConfig.Height;
            var cellSize = Mathf.Min(byWidth, byHeight);

            var frameWidth  = _gridConfig.Width  * cellSize;
            var frameHeight = _gridConfig.Height * cellSize + _boardConfig.FrameExtraHeight;
            return (frameWidth, frameHeight, cellSize);
        }

        private float ComputeTileCellSize(Camera cam)
        {
            var camHeight = cam.orthographicSize * 2f;
            var camWidth = camHeight * GetAspect(cam);
            var effectiveWidth = Mathf.Min(camWidth, camHeight * _boardConfig.MaxAspectRatio);

            var byWidth = effectiveWidth * (1f - _boardConfig.TilePaddingPercent) / _gridConfig.Width;
            var byHeight = camHeight * (1f - _boardConfig.UIReservedHeightPercent) / _gridConfig.Height;

            return Mathf.Min(byWidth, byHeight);
        }

        private static float GetAspect(Camera cam)
        {
            var h = UnityEngine.Device.Screen.height;
            
            return h > 0 ? (float)UnityEngine.Device.Screen.width / h : cam.aspect;
        }
    }
}