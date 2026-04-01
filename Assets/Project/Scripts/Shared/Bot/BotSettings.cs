namespace Project.Scripts.Shared.Bot
{
    public readonly struct BotSettings
    {
        public readonly float MinDischargeDelay;
        public readonly float MaxDischargeDelay;


        public BotSettings(float minDischargeDelay, float maxDischargeDelay)
        {
            MinDischargeDelay = minDischargeDelay;
            MaxDischargeDelay = maxDischargeDelay;
        }
    }
}