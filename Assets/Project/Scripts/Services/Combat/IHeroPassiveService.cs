using System.Collections.Generic;
using Project.Scripts.Shared.Passives;

namespace Project.Scripts.Services.Combat
{
    public interface IHeroPassiveService
    {
        IReadOnlyList<HeroPassiveRuntimeState> States { get; }
    }
}