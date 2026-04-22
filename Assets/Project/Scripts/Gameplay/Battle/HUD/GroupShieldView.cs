using R3;
using UnityEngine;

namespace Project.Scripts.Gameplay.Battle.HUD
{
    public class GroupShieldView : MonoBehaviour
    {
        private CompositeDisposable _disposables;


        private void OnDestroy()
        {
            _disposables?.Dispose();
        }


        public void Bind(Observable<bool> isGroupAlive)
        {
            _disposables?.Dispose();
            _disposables = new CompositeDisposable();
            isGroupAlive
                .Subscribe(alive => gameObject.SetActive(alive))
                .AddTo(_disposables);
        }
    }
}