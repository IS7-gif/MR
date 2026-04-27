namespace Project.Scripts.Shared.Passives
{
    public enum PassiveAbilityKind
    {
        None,
        MatchEnergyBySlotKindPercent,
        SpecialTileEnergyPercent,
        EnergyCostMultiplierAndPowerBonus,
        EnergyCostReduction,
        RepeatAbilityOnAdditionalTargets,
        RepeatAbilityOnSameTarget,
        RepeatLineSpecialOnAdjacentLines,
        BombRadiusBonus,
        ResurrectOnDeath,
        GrantTeamAbilityPowerUntilNextActivation
    }
}