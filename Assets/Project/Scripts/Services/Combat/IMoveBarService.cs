using Project.Scripts.Shared.Moves;

namespace Project.Scripts.Services.Combat
{
    public interface IMoveBarService
    {
        bool HasMoves { get; }
        MoveBarSnapshot GetSnapshot();
        void Initialize();
        void Tick(float deltaTime);
        bool TryConsume();
    }
}