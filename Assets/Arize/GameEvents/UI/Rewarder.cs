using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Yurowm.UltimateMatchThree.Scripts.Assistants;
using Ninsar.GameEvents.Quests;
using Ninsar.GameEvents.Quests.Rewards;
using UnityEngine;
using Yurowm.GameCore;
using Random = UnityEngine.Random;

namespace Ninsar.GameEvents.UI
{
    public class Rewarder : MonoBehaviour
    {
        [ContentSelector]
        public CollectionEffect CollectionEffect;

        public Transform Position;
        
        private void OnEnable()
        {
            Quest.OnGettedRewardItems += OnGettedRewardItems;
        }
        
        private void OnDisable()
        {
            Quest.OnGettedRewardItems -= OnGettedRewardItems;
        }

        private void OnGettedRewardItems(Dictionary<Reward.Item, int> items)
        {
            foreach (var itemAndCount in items)
            {
                if (itemAndCount.Key == null) continue;
                
                switch (itemAndCount.Key)
                {
                    case Reward.InventoryItem inventoryItem:
                        StartCoroutine(EmitEffect(Position.position, ItemIcons.main.GetIconOrNull(inventoryItem.Item), Math.Min(itemAndCount.Value, 3)));
                        break;
                    case Reward.BerryStoreItem berryStoreItem:
                        if (berryStoreItem.Item != null)
                        {
                            foreach (var item in berryStoreItem.Item.icons)
                            {
                                StartCoroutine(EmitEffect(Position.position, item.icon, Math.Min(itemAndCount.Value, 3)));
                            }
                        }
                        break;
                }
            }
        }
        
        private IEnumerator EmitEffect(Vector3 position, Sprite icon, int count)
        {
            if (!CollectionEffect) yield break;
            
            var targets = ObjectTag.GetFirst("BottomDeep").transform;
            
            yield return new WaitForSeconds(Random.Range(0, 0.05f));

            for (var i = 0; i < count; i++) {
                var effect = Content.Emit(CollectionEffect);
                effect.transform.Reset();
                effect.transform.position = position;
                effect.SetIcon(icon, 0.5f);
                effect.SetOrder(count - i);
                effect.SetTarget(targets);
                effect.Play();
                    
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}