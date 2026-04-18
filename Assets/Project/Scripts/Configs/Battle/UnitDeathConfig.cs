using System;
using UnityEngine;

namespace Project.Scripts.Configs.Battle
{
    [CreateAssetMenu(fileName = "UnitDeathConfig", menuName = "Configs/Battle/Unit Death Config")]
    public class UnitDeathConfig : ScriptableObject
    {
        [Tooltip("Настройки визуального состояния героя при гибели.")]
        [SerializeField] private HeroDeathVisuals _heroDeathVisuals;

        [Space(10)]
        [Tooltip("Настройки визуального состояния аватара при гибели.")]
        [SerializeField] private AvatarDeathVisuals _avatarDeathVisuals;


        public HeroDeathVisuals HeroDeathVisuals => _heroDeathVisuals;
        public AvatarDeathVisuals AvatarDeathVisuals => _avatarDeathVisuals;


        private void Reset()
        {
            _heroDeathVisuals = new HeroDeathVisuals
            {
                ApplyDeathFill = true,
                DeathColor = new Color(0.3f, 0.3f, 0.3f, 1f)
            };
            _avatarDeathVisuals = new AvatarDeathVisuals
            {
                ApplyDeathFill = true,
                DeathColor = new Color(0.298f, 0.298f, 0.298f, 1f)
            };
        }
    }

    [Serializable]
    public struct HeroDeathVisuals
    {
        [Tooltip("Применять шейдерную заливку портрета при гибели героя.")]
        public bool ApplyDeathFill;

        [Tooltip("Цвет, в который красятся _deathColoredRenderers при гибели героя.")]
        public Color DeathColor;
    }

    [Serializable]
    public struct AvatarDeathVisuals
    {
        [Tooltip("Применять шейдерную заливку портрета при гибели аватара.")]
        public bool ApplyDeathFill;

        [Tooltip("Цвет, в который красятся _deathColoredRenderers при гибели аватара.")]
        public Color DeathColor;
    }
}