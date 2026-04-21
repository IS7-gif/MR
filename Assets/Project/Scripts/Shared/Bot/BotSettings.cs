namespace Project.Scripts.Shared.Bot
{
    public readonly struct BotSettings
    {
        public readonly float MinAvatarActivationDelay;
        public readonly float MaxAvatarActivationDelay;


        public BotSettings(float minAvatarActivationDelay, float maxAvatarActivationDelay)
        {
            MinAvatarActivationDelay = minAvatarActivationDelay;
            MaxAvatarActivationDelay = maxAvatarActivationDelay;
        }
    }
}