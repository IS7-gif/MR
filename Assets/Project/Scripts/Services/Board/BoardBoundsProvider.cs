namespace Project.Scripts.Services.Board
{
    public class BoardBoundsProvider : IBoardBoundsProvider
    {
        public float BoardTopWorldY { get; private set; }
        public float BoardHalfWidth { get; private set; }
        public float BoardCenterX { get; private set; }
        public float CellSize { get; private set; }
        public float AnnouncementAnchorWorldY { get; private set; }


        public void SetBounds(float centerX, float topWorldY, float halfWidth, float cellSize)
        {
            BoardCenterX = centerX;
            BoardTopWorldY = topWorldY;
            BoardHalfWidth = halfWidth;
            CellSize = cellSize;
        }

        public void SetAnnouncementAnchorY(float worldY)
        {
            AnnouncementAnchorWorldY = worldY;
        }
    }
}