using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Ninsar.GameEvents.Quests
{
    public class QuestGroupTracker : MonoBehaviour
    {
        public Image Ping;
        
        [SerializeField]
        private List<Quest> _quests;
        
        private void OnEnable()
        {
            Quest.OnQuestsReady += OnQuestsUpdate;
            Quest.OnQuestsComplete += OnQuestsUpdate;
            
            UpdateInfo();
        }

        private void OnDisable()
        {
            Quest.OnQuestsReady -= OnQuestsUpdate;
            Quest.OnQuestsComplete -= OnQuestsUpdate;
        }

        private void OnQuestsUpdate(Quest quest)
        {
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            Ping.gameObject.SetActive(AnyQuestIsReady);
        }

        private bool AnyQuestIsReady => _quests.Any(x => x.IsPinging());
    }
}
