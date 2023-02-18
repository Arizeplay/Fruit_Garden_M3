using System.Linq;
using UnityEngine;
namespace Ninsar.GameEvents.Quests.Rewards
{
    [CreateAssetMenu(menuName = "Create Quest Reward/Sale", fileName = "QuestRewardSale", order = 0)]
    public class RewardSale : Reward
    {
        public string SaleItemId;
        
        public override (Item, int) Apply()
        {
            var item = BerryStore.main.items.FirstOrDefault(x => x.id == SaleItemId);
            if (item == null) return (null, 0);
            
            //item.Sale;
            
            return (new BerryStoreItem(item), 1);
        }
    }
}