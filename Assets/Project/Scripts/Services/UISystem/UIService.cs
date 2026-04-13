using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Project.Scripts.Configs;
using UnityEngine;
using VContainer;
using ZLinq;

namespace Project.Scripts.Services.UISystem
{
    public class UIService : MonoBehaviour
    {
        [Tooltip("Background layer canvas (Sort Order 0). For background screens.")]
        [SerializeField] private Canvas _backgroundCanvas;
        
        [Tooltip("Main static layer canvas (Sort Order 100). For static HUD elements: buttons, labels.")]
        [SerializeField] private Canvas _mainCanvas;
        
        [Tooltip("Main dynamic layer canvas (Sort Order 150). For animated elements: avatars, heroes, HP bars, effects.")]
        [SerializeField] private Canvas _mainDynamicCanvas;
        
        [Tooltip("Popup layer canvas (Sort Order 200). For popups and overlays.")]
        [SerializeField] private Canvas _popupCanvas;
        
        [Tooltip("System layer canvas (Sort Order 300). For system UI: loading screens, alerts.")]
        [SerializeField] private Canvas _systemCanvas;

        
        private readonly Dictionary<Type, GameObject> _registeredViews = new();
        private readonly Dictionary<Type, IView> _activeViews = new();
        private readonly Dictionary<Type, UILayer> _viewLayers = new();
        private DebugConfig _debugConfig;


        private void Awake()
        {
            SetupCanvasLayers();
        }


        [Inject]
        public void Construct(DebugConfig debugConfig)
        {
            _debugConfig = debugConfig;
        }


        public void RegisterView<TView>(GameObject prefab, UILayer layer) where TView : MonoBehaviour, IView
        {
            var type = typeof(TView);
            _registeredViews[type] = prefab;
            _viewLayers[type] = layer;
            if (_debugConfig.LogUIEvents)
                Debug.Log($"View {type.Name} registered on layer {layer}");
        }

        public async UniTask<TView> Show<TView, TViewModel>(TViewModel viewModel)
            where TView : BaseView<TViewModel>
            where TViewModel : BaseViewModel
        {
            var viewType = typeof(TView);

            if (_activeViews.TryGetValue(viewType, out var existingView))
            {
                var typedView = existingView as TView;
                if (!typedView)
                {
                    Debug.LogError($"Active view is not of type {viewType.Name}, removing corrupted entry");
                    _activeViews.Remove(viewType);
                }
                else
                {
                    await typedView.ShowAsync();
                    return typedView;
                }
            }

            if (false == _registeredViews.TryGetValue(viewType, out var prefab))
            {
                Debug.LogError($"View {viewType.Name} not registered!");
                return null;
            }

            var viewObject = Instantiate(prefab, GetParentForLayer(_viewLayers[viewType]));
            var view = viewObject.GetComponent<TView>();
            if (!view)
            {
                Debug.LogError($"Prefab doesn't have {viewType.Name} component!");
                Destroy(viewObject);
                return null;
            }

            await view.InitializeAsync(viewModel);
            await view.ShowAsync();

            _activeViews[viewType] = view;

            if (_debugConfig.LogUIEvents)
                Debug.Log($"View {viewType.Name} shown");
            return view;
        }

        public async UniTask<TView> Show<TView, TViewModel>()
            where TView : BaseView<TViewModel>
            where TViewModel : BaseViewModel, new()
        {
            var viewType = typeof(TView);

            if (_activeViews.TryGetValue(viewType, out var existingView))
            {
                var typedView = existingView as TView;
                if (!typedView)
                {
                    Debug.LogError($"Active view is not of type {viewType.Name}, removing corrupted entry");
                    _activeViews.Remove(viewType);
                }
                else
                {
                    await typedView.ShowAsync();
                    return typedView;
                }
            }

            if (false == _registeredViews.TryGetValue(viewType, out var prefab))
            {
                Debug.LogError($"View {viewType.Name} not registered!");
                return null;
            }

            var viewObject = Instantiate(prefab, GetParentForLayer(_viewLayers[viewType]));
            var view = viewObject.GetComponent<TView>();
            if (!view)
            {
                Debug.LogError($"Prefab doesn't have {viewType.Name} component!");
                Destroy(viewObject);
                return null;
            }

            var viewModel = new TViewModel();
            await view.InitializeAsync(viewModel);
            await view.ShowAsync();

            _activeViews[viewType] = view;

            if (_debugConfig.LogUIEvents)
                Debug.Log($"View {viewType.Name} shown");
            return view;
        }

        public async UniTask Hide<TView>() where TView : MonoBehaviour, IView
        {
            var viewType = typeof(TView);

            if (false == _activeViews.TryGetValue(viewType, out var view))
            {
                if (_debugConfig.LogUIEvents)
                Debug.LogWarning($"View {viewType.Name} is not active");
                return;
            }

            await view.HideAsync();
        }

        public void Close<TView>() where TView : MonoBehaviour, IView
        {
            var viewType = typeof(TView);

            if (false == _activeViews.Remove(viewType, out var view))
                return;

            if (view is MonoBehaviour mono && mono)
                view.Close();
        }

        public TView GetCurrent<TView>() where TView : MonoBehaviour, IView
        {
            var viewType = typeof(TView);

            if (_activeViews.TryGetValue(viewType, out var view))
                return view as TView;

            return null;
        }

        public void CloseAll()
        {
            var types = _activeViews.Keys.AsValueEnumerable().ToList();

            for (var i = 0; i < types.Count; i++)
            {
                var view = _activeViews[types[i]];
                if (view is MonoBehaviour mono && mono)
                    view.Close();
            }

            _activeViews.Clear();
        }


        private void SetupCanvasLayers()
        {
            SetupCanvas(_backgroundCanvas, UILayer.Background);
            SetupCanvas(_mainCanvas, UILayer.Main);
            SetupCanvas(_mainDynamicCanvas, UILayer.MainDynamic);
            SetupCanvas(_popupCanvas, UILayer.Popup);
            SetupCanvas(_systemCanvas, UILayer.System);
        }

        private void SetupCanvas(Canvas canvas, UILayer layer)
        {
            if (!canvas)
            {
                Debug.LogError($"Canvas for layer {layer} is not assigned!");
                return;
            }

            canvas.sortingOrder = (int)layer;
        }

        private Transform GetParentForLayer(UILayer layer)
        {
            return GetCanvasForLayer(layer).transform;
        }

        public Transform GetLayerRoot(UILayer layer)
        {
            return GetCanvasForLayer(layer).transform;
        }

        private Canvas GetCanvasForLayer(UILayer layer)
        {
            return layer switch
            {
                UILayer.Background => _backgroundCanvas,
                UILayer.Main => _mainCanvas,
                UILayer.MainDynamic => _mainDynamicCanvas,
                UILayer.Popup => _popupCanvas,
                UILayer.System => _systemCanvas,
                _ => _mainCanvas
            };
        }
    }
}