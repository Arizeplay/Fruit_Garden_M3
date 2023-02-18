using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ninsar.GameEvents.Quests
{
    public enum RarityType {Gray, Green, Blue, Purple, Orange}

    [Serializable]
    public class Rarity
    {
        public RarityType Type;
        public int FromIndex;
        public int ToIndex;
        
        public static RarityType GetRarity(List<Rarity> rarities, int index)
        {
            var rarity = rarities.FirstOrDefault(x => index >= x.FromIndex  && index < x.ToIndex) ?? rarities.Last();
            
            return rarity.Type;
        } 
        
        public static int GetPosition(List<Rarity> rarities, int index, out int maxCount)
        {
            var rarity = rarities.FirstOrDefault(x => index >= x.FromIndex && index < x.ToIndex) ?? rarities.Last();

            maxCount = rarity.ToIndex - rarity.FromIndex;
            
            return index - rarity.FromIndex;
        }
    }
}