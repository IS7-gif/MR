using System;

namespace Project.Scripts.Services
{
    public interface ISwapInputHandler
    {
        event Action<SwapRequest> OnSwapRequested;
    }
}