using Cysharp.Threading.Tasks;

namespace Project.Scripts.Services.Announcements
{
    public interface IBoardAnnouncementService
    {
        UniTask Show(string text, BoardAnnouncementParams @params = null);
    }
}