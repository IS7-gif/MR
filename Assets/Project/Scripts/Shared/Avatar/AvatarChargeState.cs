namespace Project.Scripts.Shared.Avatar
{
    public readonly struct AvatarChargeState
    {
        public int CurrentCharge { get; }
        public int MaxCharge { get; }
        public bool IsFull => MaxCharge > 0 && CurrentCharge >= MaxCharge;
        public bool IsEmpty => CurrentCharge <= 0;
        public float FillFraction => MaxCharge > 0 ? (float)CurrentCharge / MaxCharge : 0f;


        public AvatarChargeState(int currentCharge, int maxCharge)
        {
            CurrentCharge = currentCharge;
            MaxCharge = maxCharge;
        }
    }
}