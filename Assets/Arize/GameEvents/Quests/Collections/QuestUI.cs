using System;
using Ninsar.GameEvents.UI;
using UnityEngine;
using Object = UnityEngine.Object;
namespace Ninsar.GameEvents.Quests.Collections
{
    [Serializable]
    public class QuestUI : QuestComponent
    {
        [SerializeField]
        private QuestShortPanel _prefab;
        private QuestShortPanel _questShortPanel;
        
        public override GameObject Instantiate(Quest quest, Transform parent)
        {
            _questShortPanel = Object.Instantiate(_prefab, parent);
            _questShortPanel.Quest = quest;
            _questShortPanel.Init();
            return _questShortPanel.gameObject;
        }
        
        public override void Update()
        {
            _questShortPanel.UpdateInfo();
        }
    }

}