using Project.Scripts.Shared.Heroes;

namespace Project.Scripts.Shared.Passives
{
    public enum PassiveUnitTargetTeam
    {
        None,
        Allies,
        Enemies,
        Both
    }

    public enum PassiveUnitTargetKind
    {
        None,
        Units,
        Heroes,
        Avatar
    }

    public enum PassiveUnitSelectionMode
    {
        None,
        All,
        RandomOne,
        RandomCount,
        MostWounded
    }

    public readonly struct PassiveUnitTargetDefinition
    {
        public PassiveUnitTargetTeam Team { get; }
        public PassiveUnitTargetKind UnitKind { get; }
        public PassiveUnitSelectionMode SelectionMode { get; }
        public int Count { get; }
        public bool ExcludeOwner { get; }

        public bool IsConfigured =>
            Team != PassiveUnitTargetTeam.None
            && UnitKind != PassiveUnitTargetKind.None
            && SelectionMode != PassiveUnitSelectionMode.None;


        public PassiveUnitTargetDefinition(PassiveUnitTargetTeam team, PassiveUnitTargetKind unitKind,
            PassiveUnitSelectionMode selectionMode, int count, bool excludeOwner)
        {
            Team = team;
            UnitKind = unitKind;
            SelectionMode = selectionMode;
            Count = count < 0 ? 0 : count;
            ExcludeOwner = excludeOwner;
        }
    }

    public readonly struct PassiveUnitTargetCandidate
    {
        public UnitDescriptor Descriptor { get; }
        public int CurrentHP { get; }
        public int MaxHP { get; }
        public bool IsAvailable { get; }


        public PassiveUnitTargetCandidate(UnitDescriptor descriptor, int currentHP, int maxHP, bool isAvailable)
        {
            Descriptor = descriptor;
            CurrentHP = currentHP < 0 ? 0 : currentHP;
            MaxHP = maxHP < 0 ? 0 : maxHP;
            IsAvailable = isAvailable;
        }
    }
}