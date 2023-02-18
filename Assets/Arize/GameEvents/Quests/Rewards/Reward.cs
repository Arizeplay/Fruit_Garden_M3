using UnityEngine;
namespace Ninsar.GameEvents.Quests.Rewards
{
    public abstract class Reward : ScriptableObject
    {
        public abstract class Item { }
        
        public class InventoryItem : Item
        {
            public ItemID Item;
            public InventoryItem(ItemID item)
            {
                Item = item;
            }
        }
        
        public class BerryStoreItem : Item
        {
            public BerryStore.Item Item;
            public BerryStoreItem(BerryStore.Item item)
            {
                Item = item;
            }
        }

        public abstract (Item, int) Apply();
    }
}