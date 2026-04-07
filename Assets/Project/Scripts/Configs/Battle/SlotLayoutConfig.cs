using Project.Scripts.Shared.Tiles;
using UnityEngine;

namespace Project.Scripts.Configs.Battle
{
    [CreateAssetMenu(fileName = "SlotLayoutConfig", menuName = "Configs/Slot Layout Config")]
    public class SlotLayoutConfig : ScriptableObject
    {
        [Tooltip("Типы тайлов для 4 слотов героев (слева направо, не включая слот аватара)")]
        [SerializeField] private TileKind[] _heroSlotKinds = new TileKind[4];

        [Tooltip("Тип тайлов слота аватара - тайлы этого типа вносят наибольший вклад в заряд аватара")]
        [SerializeField] private TileKind _avatarSlotKind = TileKind.Void;


        public TileKind[] HeroSlotKinds => _heroSlotKinds;
        public TileKind AvatarSlotKind => _avatarSlotKind;
    }
}