using System.Collections.Generic;
using Project.Scripts.Shared.Heroes;

namespace Project.Scripts.Shared.Bot
{
    public sealed class BotDecisionEngine
    {
        private readonly BotSettings _settings;
        private readonly System.Random _rng;


        public BotDecisionEngine(BotSettings settings, int seed)
        {
            _settings = settings;
            _rng = new System.Random(seed);
        }

        public float GenerateDelay(float min, float max)
        {
            return (float)(_rng.NextDouble() * (max - min) + min);
        }

        public float GenerateDischargeDelay()
        {
            return GenerateDelay(_settings.MinDischargeDelay, _settings.MaxDischargeDelay);
        }

        public int PickRandomAssignedSlot(IReadOnlyList<HeroSlotState> slots)
        {
            var eligibleCount = 0;
            for (var i = 0; i < slots.Count; i++)
            {
                if (slots[i].CanAccumulateEnergy)
                    eligibleCount++;
            }

            if (eligibleCount == 0)
                return -1;

            var pick = _rng.Next(eligibleCount);
            var count = 0;
            for (var i = 0; i < slots.Count; i++)
            {
                if (false == slots[i].CanAccumulateEnergy)
                    continue;
                if (count == pick)
                    return i;
                count++;
            }

            return -1;
        }
    }
}