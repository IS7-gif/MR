using Project.Scripts.Configs;
using Project.Scripts.Configs.UI;
using Project.Scripts.Services.Audio.AudioSystem;
using Project.Scripts.Services.Events;
using Project.Scripts.Services.UISystem;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Project.Scripts.DI
{
    public class ProjectLifetimeScope : LifetimeScope
    {
        [SerializeField] private MainConfig _mainConfig;


        protected override void Configure(IContainerBuilder builder)
        {
            var gameResultSequenceConfig = _mainConfig.GameResultSequenceConfig
                ? _mainConfig.GameResultSequenceConfig
                : ScriptableObject.CreateInstance<GameResultSequenceConfig>();

            builder.RegisterInstance(_mainConfig.BoardConfig);
            builder.RegisterInstance(_mainConfig.BoardAnimationConfig);
            builder.RegisterInstance(_mainConfig.BattleAnimationConfig);
            builder.RegisterInstance(gameResultSequenceConfig);
            builder.RegisterInstance(_mainConfig.InputConfig);
            builder.RegisterInstance(_mainConfig.CascadeEnergyConfig);
            builder.RegisterInstance(_mainConfig.AudioMusicConfig);
            builder.RegisterInstance(_mainConfig.AudioSFXConfig);
            builder.RegisterInstance(_mainConfig.SpecialTileConfig);
            builder.RegisterInstance(_mainConfig.LevelDatabase);
            builder.RegisterInstance(_mainConfig.UIConfig);
            builder.RegisterInstance(_mainConfig.BoardAnnouncementConfig);
            builder.RegisterInstance(_mainConfig.MoveBarConfig);
            builder.RegisterInstance(_mainConfig.BattleViewConfig);
            builder.RegisterInstance(_mainConfig.TileKindPaletteConfig);
            builder.RegisterInstance(_mainConfig.SlotLayoutConfig);
            builder.RegisterInstance(_mainConfig.BattleTimerConfig);
            builder.RegisterInstance(_mainConfig.UnitDeathConfig);
            builder.RegisterInstance(_mainConfig.AutoEnergyConfig);
            builder.RegisterInstance(_mainConfig.EscalationConfig);
            builder.RegisterInstance(_mainConfig.HintConfig);
            builder.RegisterInstance(_mainConfig.DebugConfig);

            builder.Register<EventBus>(Lifetime.Singleton);
            builder.Register<AudioService>(Lifetime.Singleton);

            builder.RegisterComponentInHierarchy<AudioManager>();
            builder.RegisterComponentInHierarchy<UIService>();
        }
    }
}