namespace Project.Scripts.Services.Board
{
    public interface IBoardBoundsProvider
    {
        float BoardTopWorldY { get; }
        float BoardHalfWidth { get; }
        float BoardCenterX { get; }
        float CellSize { get; }
        float AnnouncementAnchorWorldY { get; }

        void SetBounds(float centerX, float topWorldY, float halfWidth, float cellSize);
        void SetAnnouncementAnchorY(float worldY);
    }
}