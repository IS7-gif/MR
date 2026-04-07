using Project.Scripts.Shared.Bot;
using UnityEngine;

namespace Project.Scripts.Configs.Battle
{
    [CreateAssetMenu(fileName = "BotConfig", menuName = "Configs/Bot Config")]
    public class BotConfig : ScriptableObject
    {
        [Header("Debug")]
        [Tooltip("Снимите флажок для отключения бота во время записи аналитики; не влияет на регистрацию")]
        [SerializeField] private bool _enabled = true;

        [Header("Identity")]
        [Tooltip("Имя, отображаемое для противника в боевом UI")]
        [SerializeField] private string _opponentName = "Enemy";

        [Header("Hero Energy")]
        [Tooltip("Секунды между тиками энергии героя бота")]
        [SerializeField] private float _heroEnergyTickInterval = 0.8f;

        [Tooltip("Энергия, добавляемая к случайному слоту героя за тик")]
        [SerializeField] private int _heroEnergyPerTick = 2;

        [Header("Hero Activation")]
        [Tooltip("Минимальное количество секунд, которое бот ждёт после зарядки героя перед активацией (симулирует реакцию человека)")]
        [SerializeField] private float _minHeroActivationDelay = 1.0f;

        [Tooltip("Максимальное количество секунд ожидания бота после зарядки героя перед активацией")]
        [SerializeField] private float _maxHeroActivationDelay = 4.0f;

        [Header("Avatar Charge")]
        [Tooltip("Секунды между тиками зарядки аватара врага")]
        [SerializeField] private float _enemyChargeTickInterval = 2f;

        [Tooltip("Очки заряда, добавляемые аватару врага за тик")]
        [SerializeField] private int _enemyChargePerTick = 8;

        [Tooltip("Минимальное количество секунд ожидания бота после заполнения шкалы заряда перед разрядом (симулирует реакцию человека)")]
        [SerializeField] private float _minDischargeDelay = 0.5f;

        [Tooltip("Максимальное количество секунд ожидания бота после заполнения шкалы заряда перед разрядом")]
        [SerializeField] private float _maxDischargeDelay = 2.0f;

        public bool Enabled => _enabled;
        public string OpponentName => _opponentName;
        public float MinHeroActivationDelay => _minHeroActivationDelay;
        public float MaxHeroActivationDelay => _maxHeroActivationDelay;
        public float HeroEnergyTickInterval => _heroEnergyTickInterval;
        public int HeroEnergyPerTick => _heroEnergyPerTick;
        public float EnemyChargeTickInterval => _enemyChargeTickInterval;
        public int EnemyChargePerTick => _enemyChargePerTick;
        public float MinDischargeDelay => _minDischargeDelay;
        public float MaxDischargeDelay => _maxDischargeDelay;

        
        public BotSettings ToSettings()
        {
            return new BotSettings(_minDischargeDelay, _maxDischargeDelay);
        }
    }
}