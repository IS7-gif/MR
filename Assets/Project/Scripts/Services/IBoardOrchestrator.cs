using Cysharp.Threading.Tasks;

namespace Project.Scripts.Services
{
    public interface IBoardOrchestrator
    {
        UniTask StartGame();
    }
}
