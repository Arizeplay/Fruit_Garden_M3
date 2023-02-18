using UnityEngine;
namespace Ninsar.GameEvents.Quests
{
    [CreateAssetMenu(menuName = "Create Quest Pinger/Wheel Pinger", fileName = "WheelPinger", order = 0)]
    public class QuestWheelPinger : QuestPinger
    {
        public override bool IsPinging()
        {
            return CurrentUser.main[ItemID.spin] > 0;
        }
    }
}