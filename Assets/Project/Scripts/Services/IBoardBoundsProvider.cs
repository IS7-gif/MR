namespace Project.Scripts.Services
{
    public interface IBoardBoundsProvider
    {
        float BoardTopWorldY { get; }
        float BoardHalfWidth { get; }
        float BoardCenterX { get; }

        void SetBounds(float centerX, float topWorldY, float halfWidth);
    }
}