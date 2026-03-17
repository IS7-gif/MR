using Project.Scripts.Gameplay;
using VContainer;
using VContainer.Unity;

namespace Project.Scripts.DI
{
    public class GameplayLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<GameplayEntryPoint>();
        }
    }
}