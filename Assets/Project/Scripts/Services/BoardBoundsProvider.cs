namespace Project.Scripts.Services
{
    public class BoardBoundsProvider : IBoardBoundsProvider
    {
        public float BoardTopWorldY { get; private set; }
        public float BoardHalfWidth { get; private set; }
        public float BoardCenterX { get; private set; }

        
        public void SetBounds(float centerX, float topWorldY, float halfWidth)
        {
            BoardCenterX = centerX;
            BoardTopWorldY = topWorldY;
            BoardHalfWidth = halfWidth;
        }
    }
}