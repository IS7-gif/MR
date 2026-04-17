using System.Collections.Generic;
using Project.Scripts.Shared.GroupDefense;
using Project.Scripts.Shared.Heroes;
using Project.Scripts.Shared.Tiles;
using UnityEngine;

namespace Project.Scripts.Services.Events
{
    public readonly struct MatchPlayedEvent
    {
        public int CascadeIndex { get; }


        public MatchPlayedEvent(int cascadeIndex)
        {
            CascadeIndex = cascadeIndex;
        }
    }

    public readonly struct BombActivatedEvent
    {

    }

    public readonly struct MoveUsedEvent
    {

    }

    public readonly struct MoveCountChangedEvent
    {
        public int MovesUsed { get; }


        public MoveCountChangedEvent(int movesUsed)
        {
            MovesUsed = movesUsed;
        }
    }

    public readonly struct EnemyHPChangedEvent
    {
        public int Current { get; }
        public int Max { get; }
        public bool Silent { get; }


        public EnemyHPChangedEvent(int current, int max, bool silent = false)
        {
            Current = current;
            Max = max;
            Silent = silent;
        }
    }

    public readonly struct EnemyDefeatedEvent
    {

    }

    public readonly struct EnergyGeneratedEvent
    {
        public IReadOnlyDictionary<TileKind, float> EnergyByKind { get; }
        public IReadOnlyDictionary<TileKind, Vector3> WorldSourceByKind { get; }


        public EnergyGeneratedEvent(
            IReadOnlyDictionary<TileKind, float> energyByKind,
            IReadOnlyDictionary<TileKind, Vector3> worldSourceByKind)
        {
            EnergyByKind = energyByKind;
            WorldSourceByKind = worldSourceByKind;
        }
    }

    public readonly struct PlayerHPChangedEvent
    {
        public int Current { get; }
        public int Max { get; }
        public bool Silent { get; }


        public PlayerHPChangedEvent(int current, int max, bool silent = false)
        {
            Current = current;
            Max = max;
            Silent = silent;
        }
    }

    public readonly struct HeroHPChangedEvent
    {
        public BattleSide Side { get; }
        public int SlotIndex { get; }
        public int Current { get; }
        public int Max { get; }
        public bool Silent { get; }


        public HeroHPChangedEvent(BattleSide side, int slotIndex, int current, int max, bool silent = false)
        {
            Side = side;
            SlotIndex = slotIndex;
            Current = current;
            Max = max;
            Silent = silent;
        }
    }

    public readonly struct HeroDefeatedEvent
    {
        public BattleSide Side { get; }
        public int SlotIndex { get; }
        public TileKind SlotKind { get; }


        public HeroDefeatedEvent(BattleSide side, int slotIndex, TileKind slotKind)
        {
            Side = side;
            SlotIndex = slotIndex;
            SlotKind = slotKind;
        }
    }

    public readonly struct AvatarTileBonusActivatedEvent
    {
        public BattleSide Side { get; }
        public TileKind Kind { get; }


        public AvatarTileBonusActivatedEvent(BattleSide side, TileKind kind)
        {
            Side = side;
            Kind = kind;
        }
    }

    public readonly struct HeroEnergyChangedEvent
    {
        public BattleSide Side { get; }
        public int SlotIndex { get; }
        public int Current { get; }
        public int Max { get; }


        public HeroEnergyChangedEvent(BattleSide side, int slotIndex, int current, int max)
        {
            Side = side;
            SlotIndex = slotIndex;
            Current = current;
            Max = max;
        }
    }

    public readonly struct HeroActivatedEvent
    {
        public BattleSide Side { get; }
        public int SlotIndex { get; }
        public HeroActionType ActionType { get; }
        public int ActionValue { get; }


        public HeroActivatedEvent(BattleSide side, int slotIndex, HeroActionType actionType, int actionValue)
        {
            Side = side;
            SlotIndex = slotIndex;
            ActionType = actionType;
            ActionValue = actionValue;
        }
    }

    public readonly struct EnemyAttackEvent
    {
        public int Damage { get; }


        public EnemyAttackEvent(int damage)
        {
            Damage = damage;
        }
    }

    public readonly struct PlayerDefeatedEvent
    {

    }

    public readonly struct SwapRejectedEvent
    {

    }

    public readonly struct MoveBarChangedEvent
    {
        public int CurrentMoves { get; }
        public float FillProgress { get; }
        public bool IsAtMax { get; }


        public MoveBarChangedEvent(int currentMoves, float fillProgress, bool isAtMax)
        {
            CurrentMoves = currentMoves;
            FillProgress = fillProgress;
            IsAtMax = isAtMax;
        }
    }

    public readonly struct PlayerAvatarEnergyChangedEvent
    {
        public int Current { get; }
        public int Max { get; }


        public PlayerAvatarEnergyChangedEvent(int current, int max)
        {
            Current = current;
            Max = max;
        }
    }

    public readonly struct EnemyAvatarEnergyChangedEvent
    {
        public int Current { get; }
        public int Max { get; }


        public EnemyAvatarEnergyChangedEvent(int current, int max)
        {
            Current = current;
            Max = max;
        }
    }

    public readonly struct EnemyAvatarActivatedEvent
    {
        public HeroActionType ActionType { get; }
        public int ActionValue { get; }


        public EnemyAvatarActivatedEvent(HeroActionType actionType, int actionValue)
        {
            ActionType = actionType;
            ActionValue = actionValue;
        }
    }

    public readonly struct AbilityExecutedEvent
    {
        public UnitDescriptor Source { get; }
        public UnitDescriptor Target { get; }
        public HeroActionType ActionType { get; }
        public int Value { get; }


        public AbilityExecutedEvent(UnitDescriptor source, UnitDescriptor target, HeroActionType actionType, int value)
        {
            Source = source;
            Target = target;
            ActionType = actionType;
            Value = value;
        }
    }

    public readonly struct AvatarExposedEvent
    {
        public BattleSide Side { get; }
        public HeroGroupId DestroyedGroupId { get; }


        public AvatarExposedEvent(BattleSide side, HeroGroupId destroyedGroupId)
        {
            Side = side;
            DestroyedGroupId = destroyedGroupId;
        }
    }

    public readonly struct GameResultEvent
    {
        public BattleSide Winner { get; }
        public bool IsFlawless { get; }


        public GameResultEvent(BattleSide winner, bool isFlawless)
        {
            Winner = winner;
            IsFlawless = isFlawless;
        }
    }

    public readonly struct BattleTimerChangedEvent
    {
        public float TimeRemaining { get; }
        public bool IsEscalation { get; }


        public BattleTimerChangedEvent(float timeRemaining, bool isEscalation)
        {
            TimeRemaining = timeRemaining;
            IsEscalation = isEscalation;
        }
    }

    public readonly struct BattleEscalationReachedEvent
    {
        public float TimeRemaining { get; }


        public BattleEscalationReachedEvent(float timeRemaining)
        {
            TimeRemaining = timeRemaining;
        }
    }

    public readonly struct OvertimeStartedEvent
    {

    }

    public readonly struct EscalationModifiersAppliedEvent
    {
        public float CascadeEnergyMultiplier { get; }


        public EscalationModifiersAppliedEvent(float cascadeEnergyMultiplier)
        {
            CascadeEnergyMultiplier = cascadeEnergyMultiplier;
        }
    }

    public readonly struct AutoEnergyTickEvent
    {
        public float EnergyAmount { get; }


        public AutoEnergyTickEvent(float energyAmount)
        {
            EnergyAmount = energyAmount;
        }
    }

    public readonly struct OvertimeDrainTargetChangedEvent
    {
        public BattleSide Side { get; }
        public int TargetIndex { get; }


        public OvertimeDrainTargetChangedEvent(BattleSide side, int targetIndex)
        {
            Side = side;
            TargetIndex = targetIndex;
        }
    }
}