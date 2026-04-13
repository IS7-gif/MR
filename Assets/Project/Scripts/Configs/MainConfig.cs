using Project.Scripts.Configs.Battle;
using Project.Scripts.Configs.Board;
using Project.Scripts.Configs.Levels;
using Project.Scripts.Configs.UI;
using Project.Scripts.Services.Audio.AudioSystem.Configs;
using UnityEngine;

namespace Project.Scripts.Configs
{
    [CreateAssetMenu(fileName = "MainConfig", menuName = "Configs/MainConfig")]
    public class MainConfig : ScriptableObject
    {
        [SerializeField] private AudioMusicConfig _audioMusicConfig;
        [SerializeField] private AudioSFXConfig _audioSFXConfig;
        [SerializeField] private InputConfig _inputConfig;
        [SerializeField] private BoardConfig _boardConfig;
        [SerializeField] private BoardAnimationConfig _boardAnimationConfig;
        [SerializeField] private BattleAnimationConfig _battleAnimationConfig;
        [SerializeField] private CascadeEnergyConfig _cascadeEnergyConfig;
        [SerializeField] private SpecialTileConfig _specialTileConfig;
        [SerializeField] private LevelDatabase _levelDatabase;
        [SerializeField] private UIConfig _uiConfig;
        [SerializeField] private MoveBarConfig _moveBarConfig;
        [SerializeField] private BattleViewConfig _battleViewConfig;
        [SerializeField] private TileKindPaletteConfig _tileKindPaletteConfig;
        [SerializeField] private SlotLayoutConfig _slotLayoutConfig;
        [SerializeField] private BattleTimerConfig _battleTimerConfig;
        [SerializeField] private UnitDeathConfig _unitDeathConfig;
        [SerializeField] private DebugConfig _debugConfig;


        public AudioMusicConfig AudioMusicConfig => _audioMusicConfig;
        public AudioSFXConfig AudioSFXConfig => _audioSFXConfig;
        public InputConfig InputConfig => _inputConfig;
        public BoardConfig BoardConfig => _boardConfig;
        public BoardAnimationConfig BoardAnimationConfig => _boardAnimationConfig;
        public BattleAnimationConfig BattleAnimationConfig => _battleAnimationConfig;
        public CascadeEnergyConfig CascadeEnergyConfig => _cascadeEnergyConfig;
        public SpecialTileConfig SpecialTileConfig => _specialTileConfig;
        public LevelDatabase LevelDatabase => _levelDatabase;
        public UIConfig UIConfig => _uiConfig;
        public MoveBarConfig MoveBarConfig => _moveBarConfig;
        public BattleViewConfig BattleViewConfig => _battleViewConfig;
        public TileKindPaletteConfig TileKindPaletteConfig => _tileKindPaletteConfig;
        public SlotLayoutConfig SlotLayoutConfig => _slotLayoutConfig;
        public BattleTimerConfig BattleTimerConfig => _battleTimerConfig;
        public UnitDeathConfig UnitDeathConfig => _unitDeathConfig;
        public DebugConfig DebugConfig => _debugConfig;
    }
}