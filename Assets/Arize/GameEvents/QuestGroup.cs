using System.Collections.Generic;
using System.Linq;
using Ninsar.GameEvents.Quests;
using Ninsar.GameEvents.UI;
using UnityEngine;
namespace Ninsar.GameEvents
{
    [CreateAssetMenu(menuName = "Create Event/Game Event Group", fileName = "GameEventGroup", order = 0)]
    public class QuestGroup : GameEvent
    {
        [SerializeField]
        private QuestGroupUI _itemObject;

        [SerializeField]
        private List<Quest> _quests;

        private QuestGroupUI _questUIObject;
        
        public override void Init()
        {
            Quest.OnQuestsReady += OnOnQuestsReady;
        }
        
        private void OnOnQuestsReady(Quest quest)
        {
            if (_quests.Contains(quest))
            {
                _questUIObject.UpdateInfo();
            }
        }

        public bool AnyQuestIsReady => _quests.Any(x => x.IsReady());

        public override GameObject Instantiate(Transform parent)
        {
            _questUIObject = Instantiate(_itemObject, parent);
            _questUIObject.QuestGroup = this;
            _questUIObject.Init();
            
            return _questUIObject.gameObject;
        }
        
        public override void Update() { }
        
        public override void Load() { }
        
        public override void Save() { }
    }
}