using Cysharp.Threading.Tasks;
using R3;

namespace Project.Scripts.Services.Announcements
{
    public interface IBoardAnnouncementService
    {
        ReadOnlyReactiveProperty<bool> IsEnergyTextHidden { get; }

        UniTask Show(string text, BoardAnnouncementParams @params = null);
    }
}
