using UnityEngine;

namespace Project.Scripts.Configs
{
    [CreateAssetMenu(fileName = "WorldHUDConfig", menuName = "Configs/World HUD Config")]
    public class WorldHUDConfig : ScriptableObject
    {
        [Tooltip("World-space Y offset added above the board top edge to position the HUD root")]
        [SerializeField] private float _hudBottomPadding = 0.15f;

        
        public float HudBottomPadding => _hudBottomPadding;
    }
}