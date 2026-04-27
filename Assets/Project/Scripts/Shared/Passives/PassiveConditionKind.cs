namespace Project.Scripts.Shared.Passives
{
    public enum PassiveConditionKind
    {
        None,
        HeroActivationsInHeroPhase,
        EnergyCollectedInMatchPhase,
        MatchesCollectedInMatchPhase,
        LineSpecialUsesInBattle,
        BombUsesInBattle,
        StormUsesInBattle,
        HeroActivationsInTimeWindow,
        SlotKindMatchesInMatchPhase,
        SlotKindMatchesInTimeWindow,
        EnemyHeroDoubleKillWithinSeconds
    }
}