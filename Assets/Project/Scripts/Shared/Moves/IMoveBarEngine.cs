namespace Project.Scripts.Shared.Moves
{
    public interface IMoveBarEngine
    {
        MoveBarSnapshot Snapshot { get; }
        void Initialize(MoveBarSettings settings);
        bool Tick(float deltaTime);
        bool TryConsume();
    }
}