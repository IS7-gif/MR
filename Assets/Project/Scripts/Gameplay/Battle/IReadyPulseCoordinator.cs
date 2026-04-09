using R3;

namespace Project.Scripts.Gameplay.Battle
{
    public interface IReadyPulseCoordinator
    {
        Observable<float> Alpha { get; }
    }
}