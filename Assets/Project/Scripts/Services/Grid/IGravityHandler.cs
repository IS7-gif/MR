using Cysharp.Threading.Tasks;

namespace Project.Scripts.Services.Grid
{
    public interface IGravityHandler
    {
        UniTask ApplyGravity();
        UniTask SpawnNewTiles();
    }
}