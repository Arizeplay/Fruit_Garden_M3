using System.Collections;
using Ninsar.GameEvents.Quests;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
namespace Ninsar.GameEvents.UI.Other
{
    public class WheelTimer : MonoBehaviour
    {
        [FormerlySerializedAs("QuestShortPlane")]
        [FormerlySerializedAs("questShortPlane")]
        [FormerlySerializedAs("QuestObject")]
        public QuestShortPanel questShortPanel;
        
        public TMP_Text Text;
        
        public float Delay;
        
        private Coroutine _timerCoroutine;

        public UnityEvent OnShowTimer;
        public UnityEvent OnHideTimer;
        
        protected void OnEnable()
        {
            _timerCoroutine = StartCoroutine(TimerCoroutine());
        }
        
        protected void OnDisable()
        {
            if (_timerCoroutine != null)
            {
                StopCoroutine(_timerCoroutine);
            }
        }

        private IEnumerator TimerCoroutine()
        {
            while (true)
            {
                var quest = questShortPanel.Quest;
                if (quest) UpdateText(quest);

                if (Delay < 0.1f)
                {
                    Delay = 0.1f;
                }
                
                yield return new WaitForSeconds(Delay);
            }
        }

        private void UpdateText(Quest quest)
        {
            if (CurrentUser.main[ItemID.spin] > 0)
            {
                Text.text = quest.CurrentTargets + "/" + quest.CurrentLevel.TargetItems;
                OnHideTimer.Invoke();
            }
            else
            {
                if (TrueTime.IsKnown)
                {
                    Text.text = CurrentUser.main.dailySpin.GetTimer();
                    OnShowTimer.Invoke();
                }
            }
        }
    }
}