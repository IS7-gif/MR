using Project.Scripts.Shared.Layout;
using UnityEngine;

namespace Project.Scripts.Services.Layout
{
    public interface IGameplayScreenLayoutService
    {
        GameplayScreenLayout Calculate();
        Rect ToUnityRect(ScreenLayoutRect rect);
        Rect ToWorldRect(Camera camera, ScreenLayoutRect rect);
    }
}