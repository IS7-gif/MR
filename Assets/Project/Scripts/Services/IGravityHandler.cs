using Cysharp.Threading.Tasks;
using Project.Scripts.SpawnRules;

namespace Project.Scripts.Services
{
    public interface IGravityHandler
    {
        UniTask ApplyGravity();
        UniTask SpawnNewTiles(SpawnContext context);
    }
}
