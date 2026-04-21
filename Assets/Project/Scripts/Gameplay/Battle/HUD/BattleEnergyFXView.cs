using System;
using System.Collections.Generic;
using Project.Scripts.Configs.Battle;
using Project.Scripts.Gameplay.Battle.FX;
using Project.Scripts.Gameplay.Battle.Units;
using Project.Scripts.Services.Events;
using Project.Scripts.Shared;
using Project.Scripts.Shared.Tiles;
using Project.Scripts.Utils;
using UnityEngine;
using UnityEngine.Pool;

namespace Project.Scripts.Gameplay.Battle.HUD
{
    public class BattleEnergyFXView : MonoBehaviour
    {
        private const int DefaultPoolCapacity = 4;
        private const int MaxPoolSize = 16;


        [Tooltip("Prefab with EnergyTransferView component")]
        [SerializeField] private EnergyTransferView _energyTransferPrefab;

        [Tooltip("Optional container for spawned energy transfer instances")]
        [SerializeField] private Transform _fxContainer;


        private readonly Dictionary<TileKind, TargetBinding> _targetsByKind = new();
        private BattleAnimationConfig _animationConfig;
        private IDisposable _subscription;
        private ObjectPool<EnergyTransferView> _transferPool;
        private AvatarSlotView _playerAvatarSlot;
        private Color _avatarColor;


        private void OnDestroy()
        {
            Cleanup();
        }


        public void Initialize(
            EventBus eventBus,
            HeroSlotView[] playerHeroSlots,
            HeroSlotViewModel[] playerHeroViewModels,
            TileKind[] playerHeroKinds,
            AvatarSlotView playerAvatarSlot,
            Color avatarColor,
            BattleAnimationConfig animationConfig)
        {
            Cleanup();

            if (false == _energyTransferPrefab || null == eventBus || null == animationConfig)
                return;

            _animationConfig = animationConfig;
            _playerAvatarSlot = playerAvatarSlot;
            _avatarColor = avatarColor;
            BuildTargets(playerHeroSlots, playerHeroViewModels, playerHeroKinds);

            var container = _fxContainer ? _fxContainer : transform;
            _transferPool = new ObjectPool<EnergyTransferView>(
                createFunc: () => Instantiate(_energyTransferPrefab, container),
                actionOnGet: transfer => transfer.gameObject.SetActive(true),
                actionOnRelease: transfer =>
                {
                    transfer.Kill();
                    transfer.gameObject.SetActive(false);
                },
                actionOnDestroy: transfer =>
                {
                    if (transfer)
                        Destroy(transfer.gameObject);
                },
                defaultCapacity: DefaultPoolCapacity,
                maxSize: MaxPoolSize);

            _subscription = eventBus.Subscribe<EnergyGeneratedVisualEvent>(OnEnergyGenerated);
        }

        public void Cleanup()
        {
            _subscription?.Dispose();
            _subscription = null;
            _transferPool?.Dispose();
            _transferPool = null;
            _targetsByKind.Clear();
            _animationConfig = null;
            _playerAvatarSlot = null;
        }


        private void OnEnergyGenerated(EnergyGeneratedVisualEvent e)
        {
            if (null == _transferPool)
                return;

            foreach (var pair in e.SourceByKind)
            {
                if (false == _targetsByKind.TryGetValue(pair.Key, out var target))
                    continue;

                if (false == target.IsValid)
                    continue;

                var transfer = _transferPool.Get();
                transfer.Play(
                    pair.Value.ToUnityVector3(),
                    target.View.EnergyAnchor.position,
                    target.ViewModel.SlotColor,
                    _animationConfig,
                    () => _transferPool.Release(transfer));
            }

            if (_playerAvatarSlot && e.SourceByKind.Count > 0)
            {
                var centroid = ComputeCentroid(e.SourceByKind);
                var avatarTransfer = _transferPool.Get();
                avatarTransfer.Play(
                    centroid,
                    _playerAvatarSlot.EnergyAnchor.position,
                    _avatarColor,
                    _animationConfig,
                    () => _transferPool.Release(avatarTransfer));
            }
        }

        private static Vector3 ComputeCentroid(IReadOnlyDictionary<TileKind, SharedVector3> positions)
        {
            var sum = Vector3.zero;
            foreach (var pair in positions)
                sum += pair.Value.ToUnityVector3();
            
            return sum / positions.Count;
        }

        private void BuildTargets(HeroSlotView[] playerHeroSlots, HeroSlotViewModel[] playerHeroViewModels, TileKind[] playerHeroKinds)
        {
            if (null == playerHeroSlots || null == playerHeroViewModels || null == playerHeroKinds)
                return;

            var count = Mathf.Min(playerHeroSlots.Length, playerHeroViewModels.Length, playerHeroKinds.Length);
            for (var i = 0; i < count; i++)
            {
                var view = playerHeroSlots[i];
                var viewModel = playerHeroViewModels[i];
                if (false == view || null == viewModel)
                    continue;

                _targetsByKind[playerHeroKinds[i]] = new TargetBinding(view, viewModel);
            }
        }


        private readonly struct TargetBinding
        {
            public HeroSlotView View { get; }
            public HeroSlotViewModel ViewModel { get; }
            public bool IsValid => View && null != ViewModel && ViewModel.IsAssigned && false == ViewModel.IsDefeated.CurrentValue;


            public TargetBinding(HeroSlotView view, HeroSlotViewModel viewModel)
            {
                View = view;
                ViewModel = viewModel;
            }
        }
    }
}