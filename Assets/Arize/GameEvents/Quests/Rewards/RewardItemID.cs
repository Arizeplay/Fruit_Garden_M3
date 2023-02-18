using UnityEngine;
namespace Ninsar.GameEvents.Quests.Rewards
{
    [CreateAssetMenu(menuName = "Create Quest Reward/ItemID", fileName = "QuestRewardItemID", order = 0)]
    public class RewardItemID : Reward
    {
        public ItemID ItemID;
        public int Count;
        
        public override (Item, int) Apply()
        {
            CurrentUser.main[ItemID] += Count;
            
            return (new InventoryItem(ItemID), Count);
        }
    }
}