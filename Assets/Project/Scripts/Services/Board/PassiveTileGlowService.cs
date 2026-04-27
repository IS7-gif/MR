using System;
using System.Collections.Generic;
using Project.Scripts.Configs.Board;
using Project.Scripts.Configs.Grid;
using Project.Scripts.Services.Combat;
using Project.Scripts.Services.Events;
using Project.Scripts.Services.Grid;
using Project.Scripts.Shared;
using Project.Scripts.Shared.Heroes;
using Project.Scripts.Shared.Passives;
using Project.Scripts.Shared.Tiles;
using UnityEngine;

namespace Project.Scripts.Services.Board
{
    public class PassiveTileGlowService : IDisposable
    {
        private readonly IGridView _gridView;
        private readonly GridConfig _gridConfig;
        private readonly IHeroPassiveService _heroPassiveService;
        private readonly TileKindPaletteConfig _palette;
        private readonly IDisposable _passiveActivatedSubscription;
        private readonly IDisposable _passiveDisabledSubscription;
        private readonly IDisposable _moveUsedSubscription;


        public PassiveTileGlowService(
            EventBus eventBus,
            IGridView gridView,
            GridConfig gridConfig,
            IHeroPassiveService heroPassiveService,
            TileKindPaletteConfig palette)
        {
            _gridView = gridView;
            _gridConfig = gridConfig;
            _heroPassiveService = heroPassiveService;
            _palette = palette;

            _passiveActivatedSubscription = eventBus.Subscribe<HeroPassiveActivatedEvent>(_ => Refresh());
            _passiveDisabledSubscription = eventBus.Subscribe<HeroPassiveDisabledEvent>(_ => Refresh());
            _moveUsedSubscription = eventBus.Subscribe<MoveUsedEvent>(_ => Refresh());
        }

        public void Dispose()
        {
            ClearAll();
            _passiveActivatedSubscription.Dispose();
            _passiveDisabledSubscription.Dispose();
            _moveUsedSubscription.Dispose();
        }

        public void Refresh()
        {
            var activeKinds = CollectActiveBoostedKinds();

            for (var x = 0; x < _gridConfig.Width; x++)
                for (var y = 0; y < _gridConfig.Height; y++)
                {
                    var tile = _gridView.GetTile(new GridPoint(x, y));
                    if (!tile)
                        continue;

                    var active = activeKinds.Contains(tile.Kind);
                    var color = active ? _palette.GetColor(tile.Kind, Color.white) : Color.white;
                    tile.SetGlowActive(active, color);
                }
        }

        private HashSet<TileKind> CollectActiveBoostedKinds()
        {
            var result = new HashSet<TileKind>();
            var states = _heroPassiveService.States;
            for (var i = 0; i < states.Count; i++)
            {
                var state = states[i];
                if (false == IsActivePlayerMatchEnergyPassive(state))
                    continue;

                result.Add(state.SlotKind);
            }

            return result;
        }

        private static bool IsActivePlayerMatchEnergyPassive(HeroPassiveRuntimeState state)
        {
            return state.Side == BattleSide.Player
                   && state.IsActive
                   && false == state.IsDisabled
                   && state.SlotKind.IsColor()
                   && state.Definition.AbilityKind == PassiveAbilityKind.MatchEnergyBySlotKindPercent;
        }

        private void ClearAll()
        {
            for (var x = 0; x < _gridConfig.Width; x++)
                for (var y = 0; y < _gridConfig.Height; y++)
                {
                    var tile = _gridView.GetTile(new GridPoint(x, y));
                    if (tile)
                        tile.SetGlowActive(false, Color.white);
                }
        }
    }
}
