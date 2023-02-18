using Ninsar.GameEvents.Quests;
using Ninsar.GameEvents.UI.Other;
using UnityEngine;

namespace Ninsar.GameEvents.UI
{
    public class QuestRarityStars : QuestStars
    {
        public Star StarPrefab;

        [SerializeField]
        private Transform emptyParent;
        
        [SerializeField]
        private Transform starParent;
        
        private Star[] _stars;

        public int StartOrder = 2;
        
        private void Instantiate(int starsCount)
        {
            if (_stars != null)
            {
                for (var i = 0; i < _stars.Length; i++)
                {
                    Destroy(_stars[i].gameObject);
                }
            }
            
            _stars = new Star[starsCount];

            for (var i = 0; i < _stars.Length; i++)
            {
                _stars[i] = Instantiate(StarPrefab, transform);
            }
        }
        
        private void SetState(RarityType rarityType, int activeStars, int starsCount)
        {
            if (_stars == null || _stars.Length != starsCount)
            {
                Instantiate(starsCount);
            }

            starParent.gameObject.SetActive(activeStars > 0);
            emptyParent.gameObject.SetActive(starsCount - activeStars > 0);
            
            var length = _stars.Length;
            for (var index = 0; index < length; index++)
            {
                if (index < activeStars)
                {
                    _stars[index].SetIcon(rarityType);
                    _stars[index].transform.SetParent(starParent);
                }
                else
                {
                    _stars[index].SetEmpty();
                    _stars[index].transform.SetParent(emptyParent);
                }
                _stars[index].SetHighlight(index == activeStars - 1);
            }
        }

        public override void UpdateInfo(Quest quest)
        {
            var stars = quest.GetStars(out var maxStars) + (quest.IsReady() ? 1 : 0);
            var rarityType = quest.GetRarity();
            
            SetState(rarityType, stars, maxStars);
        }
        
        public override void StopUpdate() { }
    }
}