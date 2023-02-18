using UnityEngine.UI;

namespace Ninsar.GameEvents.UI
{
    public class QuestGroupUI : GameEventPanel
    {
        public Image Ping;

        public QuestGroup QuestGroup { get; set; }

        protected virtual void OnEnable()
        {
            UpdateInfo();
        }
        
        public virtual void UpdateInfo()
        {
            if (Ping != null) Ping.gameObject.SetActive(QuestGroup && QuestGroup.AnyQuestIsReady);
        }
    }
}