using System;
using UnityEngine;

namespace Project.Scripts.Configs.Battle
{
    [CreateAssetMenu(fileName = "UnitDeathConfig", menuName = "Configs/Battle/Unit Death Config")]
    public class UnitDeathConfig : ScriptableObject
    {
        [SerializeField] private HeroDeathVisuals _heroDeathVisuals;
        [SerializeField] private AvatarDeathVisuals _avatarDeathVisuals;


        public HeroDeathVisuals HeroDeathVisuals => _heroDeathVisuals;
        public AvatarDeathVisuals AvatarDeathVisuals => _avatarDeathVisuals;
    }

    [Serializable]
    public struct HeroDeathVisuals
    {
        [Tooltip("Применять шейдерную заливку портрета при гибели героя.")]
        public bool ApplyDeathFill;

        [Tooltip("Скрыть портрет полностью при гибели героя.")]
        public bool DisablePortrait;

        [Tooltip("Изменить цвет фона слота на цвет смерти при гибели героя.")]
        public bool ChangeBackgroundColor;

        [Tooltip("Цвет фона слота при гибели героя.")]
        public Color DeathBackgroundColor;

        [Tooltip("Скрыть полоску HP при гибели героя.")]
        public bool DisableHpBar;

        [Tooltip("Скрыть полоску энергии при гибели героя.")]
        public bool DisableEnergyBar;
    }

    [Serializable]
    public struct AvatarDeathVisuals
    {
        [Tooltip("Применять шейдерную заливку портрета при гибели аватара.")]
        public bool ApplyDeathFill;

        [Tooltip("Скрыть портрет полностью при гибели аватара.")]
        public bool DisablePortrait;

        [Tooltip("Изменить цвет фона слота на цвет смерти при гибели аватара.")]
        public bool ChangeBackgroundColor;

        [Tooltip("Цвет фона слота при гибели аватара.")]
        public Color DeathBackgroundColor;

        [Tooltip("Скрыть полоску HP при гибели аватара.")]
        public bool DisableHpBar;

        [Tooltip("Скрыть полоску энергии при гибели аватара.")]
        public bool DisableEnergyBar;
    }
}