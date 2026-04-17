using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Project.Scripts.Configs.UI;
using Project.Scripts.Gameplay.UI.Windows;
using Project.Scripts.Services.Board;
using Project.Scripts.Services.UISystem;
using VContainer.Unity;

namespace Project.Scripts.Services.Announcements
{
    public class BoardAnnouncementService : IBoardAnnouncementService, IStartable, IDisposable
    {
        private const int InitialPoolSize = 2;


        private readonly BoardAnnouncementConfig _config;
        private readonly IBoardBoundsProvider _boardBounds;
        private readonly UIService _uiService;

        private readonly Queue<BoardAnnouncementView> _pool = new();
        private readonly List<BoardAnnouncementView> _all = new();


        public BoardAnnouncementService(
            BoardAnnouncementConfig config,
            IBoardBoundsProvider boardBounds,
            UIService uiService)
        {
            _config = config;
            _boardBounds = boardBounds;
            _uiService = uiService;
        }


        public void Start()
        {
            if (false == _config.ViewPrefab)
                return;

            for (var i = 0; i < InitialPoolSize; i++)
                _pool.Enqueue(CreateInstance());
        }

        public async UniTask Show(string text, BoardAnnouncementParams @params = null)
        {
            if (false == _config.ViewPrefab)
                return;

            var view = GetOrCreate();
            var vm = BuildViewModel(text, @params);

            await view.InitializeAsync(vm);
            await view.ShowAsync();
            await view.HideAsync();

            _pool.Enqueue(view);
        }

        public void Dispose()
        {
            for (var i = 0; i < _all.Count; i++)
            {
                if (_all[i])
                    UnityEngine.Object.Destroy(_all[i].gameObject);
            }

            _all.Clear();
            _pool.Clear();
        }


        private BoardAnnouncementViewModel BuildViewModel(string text, BoardAnnouncementParams @params)
        {
            var textColor = @params?.TextColor ?? _config.TextColor;
            var displayDuration = @params?.DisplayDuration ?? _config.DisplayDuration;
            var fadeOutDuration = @params?.FadeOutDuration ?? _config.FadeOutDuration;
            var flyDistance = @params?.FlyDistance ?? _config.FlyDistance;
            var fadeOutEase = @params?.FadeOutEase ?? _config.FadeOutEase;

            return new BoardAnnouncementViewModel(
                text,
                textColor,
                displayDuration,
                fadeOutDuration,
                flyDistance,
                fadeOutEase,
                _boardBounds.AnnouncementAnchorWorldY + _config.VerticalWorldOffset);
        }

        private BoardAnnouncementView GetOrCreate()
        {
            if (_pool.Count > 0)
                return _pool.Dequeue();

            return CreateInstance();
        }

        private BoardAnnouncementView CreateInstance()
        {
            var parent = _uiService.GetLayerRoot(UILayer.Popup);
            var go = UnityEngine.Object.Instantiate(_config.ViewPrefab, parent);
            var view = go.GetComponent<BoardAnnouncementView>();
            go.SetActive(false);
            _all.Add(view);
            
            return view;
        }
    }
}