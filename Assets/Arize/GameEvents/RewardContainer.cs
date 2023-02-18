using System;
using System.Collections.Generic;

namespace Ninsar.GameEvents
{
    [Serializable]
    public class RewardContainer
    {
        public List<RewardItem> Items = new List<RewardItem>();

        public void Obtain()
        {
            Items.ForEach(i => i.Obtain());
        }
    }

    [Serializable]
    public class RewardItem
    {
        public ItemID ItemID;
        public int Count;

        public void Obtain()
        {
            CurrentUser.main[ItemID] += Count;
        }
    }
}