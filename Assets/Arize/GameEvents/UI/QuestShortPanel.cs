using Ninsar.GameEvents.Quests;
using TMPro;
using UnityEngine.UI;

namespace Ninsar.GameEvents.UI
{
    public class QuestShortPanel : GameEventPanel
    {
        public TMP_Text Label;
        public TMP_Text Target;
        public Image Image;
        public Image Ping;

        public Quest Quest { get; set; }

        protected virtual void OnEnable()
        {
            UpdateInfo();
            
            Button.onClick.AddListener(SelectQuest);
        }

        protected virtual void OnDisable()
        {
            Button.onClick.RemoveListener(SelectQuest);
        }

        private void SelectQuest()
        {
            Quest.selected = Quest;
        }

        public virtual void UpdateInfo()
        {
            if (Ping != null) Ping.gameObject.SetActive(Quest && Quest.IsPinging());
            if (Quest == null) return;
    
            Label.text = Quest.GetLabel();
            Target.text = Quest.CurrentTargets + "/" + Quest.CurrentLevel.TargetItems;
            Target.gameObject.SetActive(!Quest.IsComplete());
            Image.sprite = Quest.GetIcon();
        }
    }

}