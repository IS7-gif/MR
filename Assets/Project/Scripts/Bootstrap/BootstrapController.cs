using System;
using Cysharp.Threading.Tasks;
using Project.Scripts.Constants;
using Project.Scripts.UI;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Scripts.Bootstrap
{
    public class BootstrapController : MonoBehaviour
    {
        [SerializeField] private LoadingScreen _loadingScreen;
        [SerializeField] private float _initialDelaySeconds = 0.1f;
        [SerializeField] private float _finalLoadingDelaySeconds = 0.3f;


        private readonly Subject<float> _progressSubject = new();


        private async void Start()
        {
            try
            {
                await StartAsync();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Critical error during bootstrap: {ex}");
            }
        }
        
        private void OnDestroy()
        {
            _progressSubject?.Dispose();
        }

        private async UniTask StartAsync()
        {
            _loadingScreen.Show();

            await UniTask.Delay((int)(_initialDelaySeconds * 1000));

            _loadingScreen.ShowProgressBar();
            _loadingScreen.SubscribeToProgress(_progressSubject);

            _progressSubject.OnNext(0f);
            await UniTask.Delay((int)(_finalLoadingDelaySeconds * 1000));
            _progressSubject.OnNext(1f);

            _loadingScreen.Hide();

            SceneManager.LoadScene(SceneNames.GamePlay, LoadSceneMode.Single);
        }
    }
}