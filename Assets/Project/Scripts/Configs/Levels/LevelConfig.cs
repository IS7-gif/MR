using Project.Scripts.Configs.Battle;
using Project.Scripts.Configs.Board;
using UnityEngine;

namespace Project.Scripts.Configs.Levels
{
    public enum LevelGoalType
    {
        DamageBased
    }

    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Configs/Level Config")]
    public class LevelConfig : ScriptableObject
    {
        [Header("Level")]
        [Tooltip("Уникальный числовой идентификатор уровня, используется для поиска в LevelDatabase")]
        [SerializeField] private int _levelId = 1;

        [Header("Board")]
        [Tooltip("Количество столбцов тайлов на доске")]
        [SerializeField] private int _width = 7;

        [Tooltip("Количество строк тайлов на доске")]
        [SerializeField] private int _height = 6;

        [Tooltip("Типы тайлов, доступные для обычных совпадений на этой доске")]
        [SerializeField] private TileConfig[] _regularTiles;

        [Tooltip("Типы тайлов, доступные как специальные (улучшенные) тайлы на этой доске")]
        [SerializeField] private TileConfig[] _specialTiles;

        [Header("Combat")]
        [Tooltip("Начальные HP аватара игрока для этого уровня")]
        [SerializeField] private int _playerHP = 550;

        [Tooltip("Начальные HP аватара противника для этого уровня")]
        [SerializeField] private int _enemyHP = 550;

        [Tooltip("Тип условия победы - DamageBased означает, что игрок побеждает, снизив HP врага до нуля")]
        [SerializeField] private LevelGoalType _goalType = LevelGoalType.DamageBased;

        // TODO: replace with player loadout from lobby
        [Tooltip("Боевые параметры аватара игрока для этого уровня")]
        [SerializeField] private AvatarCombatConfig _playerAvatarConfig;

        // TODO: replace with opponent data from matchmaking
        [Tooltip("Боевые параметры аватара противника для этого уровня")]
        [SerializeField] private AvatarCombatConfig _enemyAvatarConfig;

        // TODO: replace with lobby loadout when PvP is implemented
        [Header("Heroes (temporary - will be replaced by lobby loadout)")]
        [Tooltip("Четыре конфига героев для стороны игрока (пустой слот = null)")]
        [SerializeField] private HeroConfig[] _playerHeroes = new HeroConfig[4];

        [Tooltip("Четыре конфига героев для стороны врага; используется когда BotConfig.RandomHeroSelection = false")]
        [SerializeField] private HeroConfig[] _enemyHeroes = new HeroConfig[4];

        // TODO: replace with matchmaking opponent data when lobby is implemented
        [Header("Bot (temporary - will be replaced by lobby opponent)")]
        [Tooltip("Настройки бота для этого уровня; null означает отсутствие бота (зарезервировано для реального PvP)")]
        [SerializeField] private BotConfig _botConfig;


        public int LevelId => _levelId;
        public int Width => _width;
        public int Height => _height;
        public TileConfig[] RegularTiles => _regularTiles;
        public TileConfig[] SpecialTiles => _specialTiles;
        public int PlayerHP => _playerHP;
        public int EnemyHP => _enemyHP;
        public LevelGoalType GoalType => _goalType;
        public AvatarCombatConfig PlayerAvatarConfig => _playerAvatarConfig;
        public AvatarCombatConfig EnemyAvatarConfig => _enemyAvatarConfig;
        public HeroConfig[] PlayerHeroes => _playerHeroes;
        public HeroConfig[] EnemyHeroes => _enemyHeroes;
        public BotConfig BotConfig => _botConfig;
    }
}