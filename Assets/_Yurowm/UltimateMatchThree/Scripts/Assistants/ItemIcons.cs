using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yurowm.GameCore;

namespace _Yurowm.UltimateMatchThree.Scripts.Assistants
{
    public class ItemIcons : MonoBehaviourAssistant<ItemIcons>
    {
        [Serializable]
        public struct Icons
        {
            public ItemID ItemID;
            public Sprite Icon;
        }
        
        public List<Icons> icons;

        public Sprite GetIconOrNull(ItemID itemID)
        {
            var icon = icons.FirstOrDefault(x => x.ItemID == itemID);

            return icon.Icon;
        }
    }
}