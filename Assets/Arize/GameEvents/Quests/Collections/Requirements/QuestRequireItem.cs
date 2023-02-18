using System;
using UnityEngine;
using UnityEngine.Serialization;
namespace Ninsar.GameEvents.Quests.Collections.Requirements
{
    [Serializable]
    public class QuestRequireItem : QuestRequirement
    {
        [SerializeField]
        private ItemID _itemID;

        [FormerlySerializedAs("_difference")]
        [SerializeField]
        private Difference _collectOnDifference;
        
        public override void RegisterEvents()
        {
            CurrentUser.onInventoryUpdate += CurrentUserOnInventoryUpdate;
        }
        
        public override void UnregisterEvents()
        {
            CurrentUser.onInventoryUpdate -= CurrentUserOnInventoryUpdate;
        }

        private void CurrentUserOnInventoryUpdate(ItemID item, int currentCount, int difference)
        {
            if (item == _itemID)
            {
                if (_collectOnDifference == Difference.Negative)
                {
                    if (difference < 0)
                    {
                        OnItemAdd(Mathf.Abs(difference));
                    }
                }
                else
                {
                    if (difference > 0)
                    {
                        OnItemAdd(difference);
                    }
                }
            }
        }
    }
}