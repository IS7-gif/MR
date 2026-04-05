using Cysharp.Threading.Tasks;

namespace Project.Scripts.Services.Board
{
    public interface IBoardOrchestrator
    {
        UniTask StartGame();
    }
}