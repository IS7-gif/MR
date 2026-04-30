namespace Project.Scripts.Shared.Passives
{
    public readonly struct PassiveEffectEntryDefinition
    {
        public UnitTargetingDefinition Targeting { get; }
        public BuffDefinition Buff { get; }
        public bool IsConfigured => Buff.IsConfigured;


        public PassiveEffectEntryDefinition(UnitTargetingDefinition targeting, BuffDefinition buff)
        {
            Targeting = targeting;
            Buff = buff;
        }
    }
}