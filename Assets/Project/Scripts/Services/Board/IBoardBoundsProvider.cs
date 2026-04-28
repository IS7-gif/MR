namespace Project.Scripts.Services.Board
{
    public interface IBoardBoundsProvider
    {
        float BoardTopWorldY { get; }
        float BoardHalfWidth { get; }
        float BoardCenterX { get; }
        float CellSize { get; }
        float BattleFieldAnchorWorldY { get; }
        float EnergyBarsAnchorWorldY { get; }
        float BoardAnchorWorldY { get; }

        void SetBounds(float centerX, float topWorldY, float halfWidth, float cellSize);
        void SetBattleFieldAnchorY(float worldY);
        void SetEnergyBarsAnchorY(float worldY);
        void SetBoardAnchorY(float worldY);
    }
}