using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Project.Scripts.Configs.Battle;
using Project.Scripts.Configs.UI;
using Project.Scripts.Gameplay.UI.Windows;
using Project.Scripts.Services.Board;
using Project.Scripts.Services.UISystem;
using UnityEngine;
using VContainer.Unity;

namespace Project.Scripts.Services.Announcements
{
    public class BoardAnnouncementService : IBoardAnnouncementService, IStartable, IDisposable
    {
        private const int InitialPoolSize = 2;


        private readonly UIConfig _uiConfig;
        private readonly BattleAnimationConfig _animConfig;
        private readonly IBoardBoundsProvider _boardBounds;
        private readonly UIService _uiService;

        private readonly Queue<BoardAnnouncementView> _pool = new();
        private readonly List<BoardAnnouncementView> _all = new();


        public BoardAnnouncementService(
            UIConfig uiConfig,
            BattleAnimationConfig animConfig,
            IBoardBoundsProvider boardBounds,
            UIService uiService)
        {
            _uiConfig = uiConfig;
            _animConfig = animConfig;
            _boardBounds = boardBounds;
            _uiService = uiService;
        }


        public void Start()
        {
            if (!_uiConfig.BoardAnnouncementViewPrefab)
                return;

            for (var i = 0; i < InitialPoolSize; i++)
                _pool.Enqueue(CreateInstance());
        }

        public async UniTask Show(string text, BoardAnnouncementParams @params = null)
        {
            if (!_uiConfig.BoardAnnouncementViewPrefab)
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
            var textColor = @params?.TextColor ?? Color.white;
            var displayDuration = @params?.DisplayDuration ?? _animConfig.AnnouncementDisplayDuration;
            var fadeOutDuration = @params?.FadeOutDuration ?? _animConfig.AnnouncementFadeOutDuration;
            var flyDistance = @params?.FlyDistance ?? _animConfig.AnnouncementFlyDistance;
            var fadeOutEase = @params?.FadeOutEase ?? _animConfig.AnnouncementFadeOutEase;

            return new BoardAnnouncementViewModel(
                text,
                textColor,
                displayDuration,
                fadeOutDuration,
                flyDistance,
                fadeOutEase,
                _boardBounds.BattleAreaCenterWorldY);
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
            var go = UnityEngine.Object.Instantiate(_uiConfig.BoardAnnouncementViewPrefab, parent);
            var view = go.GetComponent<BoardAnnouncementView>();
            go.SetActive(false);
            _all.Add(view);
            return view;
        }
    }
}