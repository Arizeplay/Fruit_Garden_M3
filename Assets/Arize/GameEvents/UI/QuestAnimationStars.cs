using System.Collections;
using System.Collections.Generic;
using Ninsar.GameEvents.Quests;
using UnityEngine;

namespace Ninsar.GameEvents.UI
{
    public class QuestAnimationStars : QuestStars
    {
        public GameObject Prefab;
        public Transform Container;
    
        public float AnimationDelay = 0.5f;
        public int MaxCount = 10;
        
        private List<PanelAnimations> _stars;

        private void OnDisable()
        {
            StopUpdate();
        }

        public override void UpdateInfo(Quest quest)
        {
            StopAllCoroutines();
            StartCoroutine(ShowStars(quest));
        }

        public override void StopUpdate()
        {
            StopAllCoroutines();
        }

        private IEnumerator ShowStars(Quest quest)
        {
            var targetCount = quest.CurrentLevel.TargetItems > MaxCount ? MaxCount : quest.CurrentLevel.TargetItems;
            var currentCount = quest.CurrentLevel.TargetItems > MaxCount ?  Mathf.RoundToInt((float)quest.CurrentTargets / quest.CurrentLevel.TargetItems * MaxCount) : quest.CurrentTargets;
                
            if (_stars != null)
            {
                for (var i = 0; i < _stars.Count; i++)
                {
                    Destroy(_stars[i].gameObject);
                }
            }

            _stars = new List<PanelAnimations>();
            
            while (_stars.Count < targetCount)
            {
                var prefab = Instantiate(Prefab, Container);
                var panelAnimations = prefab.GetComponent<PanelAnimations>();
                
                _stars.Add(panelAnimations);
            }
            
            _stars.ForEach(x => x.SetVisible(false, true));
            
            for (var i = 0; i < currentCount && i < _stars.Count; i++)
            {
                _stars[i].SetVisible(true);

                yield return new WaitForSeconds(AnimationDelay);
            }
        }
    }
}