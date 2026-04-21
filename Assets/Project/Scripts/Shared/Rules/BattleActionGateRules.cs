namespace Project.Scripts.Shared.Rules
{
    public static class BattleActionGateRules
    {
        public static BattleActionGateResult Evaluate(BattleActionPhase phase, BattleActionKind actionKind)
        {
            if (phase == BattleActionPhase.MatchPhase)
            {
                return actionKind == BattleActionKind.BoardSwap
                    ? new BattleActionGateResult(BattleActionBlockReason.None)
                    : new BattleActionGateResult(BattleActionBlockReason.MatchPhase);
            }

            if (phase == BattleActionPhase.HeroPhase)
            {
                return actionKind == BattleActionKind.BoardSwap
                    ? new BattleActionGateResult(BattleActionBlockReason.HeroPhase)
                    : new BattleActionGateResult(BattleActionBlockReason.None);
            }

            if (phase == BattleActionPhase.Overtime)
                return new BattleActionGateResult(BattleActionBlockReason.Overtime);

            if (phase == BattleActionPhase.Finished)
                return new BattleActionGateResult(BattleActionBlockReason.Finished);

            return new BattleActionGateResult(BattleActionBlockReason.None);
        }
    }
    
    public readonly struct BattleActionGateResult
    {
        public bool IsAllowed => Reason == BattleActionBlockReason.None;
        public BattleActionBlockReason Reason { get; }


        public BattleActionGateResult(BattleActionBlockReason reason)
        {
            Reason = reason;
        }
    }
    
    public enum BattleActionPhase
    {
        MatchPhase,
        HeroPhase,
        Overtime,
        Finished
    }

    public enum BattleActionKind
    {
        BoardSwap,
        AbilitySourceSelect,
        AbilityCommit,
        HeroActivation,
        AvatarActivation
    }

    public enum BattleActionBlockReason
    {
        None,
        MatchPhase,
        HeroPhase,
        Overtime,
        Finished
    }
}