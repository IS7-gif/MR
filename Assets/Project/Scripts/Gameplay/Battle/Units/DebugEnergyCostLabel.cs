using TMPro;
using UnityEngine;

namespace Project.Scripts.Gameplay.Battle.Units
{
    public class DebugEnergyCostLabel : MonoBehaviour
    {
        [Tooltip("TMP text displaying energy cost for debug purposes")]
        [SerializeField] private TMP_Text _label;


        public void Show(int cost)
        {
            if (false == _label)
                return;

            _label.text = $"{cost}";
        }
    }
}