using System;
using System.Collections;
using DG.Tweening;
using Ninsar.GameEvents.Quests;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Ninsar.GameEvents.UI
{
    public class QuestCompletePanel : QuestInfo
    {
        public bool selectedLevel;
        
        public Transform Panel;

        public Vector3 PunchStrength = Vector3.one*0.05f;
        public float PunchDuration;
        
        public Button CrossButton;
        public Button CompleteButton;

        public UnityEvent OnClickButton;
        
        private bool _next;
        private bool _breakIEnumerator;
        private Action _completeAction;
        private Coroutine _coroutine;

        private void OnEnable()
        {
            _coroutine = StartCoroutine(ShowReadyQuestInfo());
            
            Quest.OnQuestsComplete += OnQuestsComplete;
        }
        
        private void OnQuestsComplete(Quest obj)
        {
            ShakePanel();
        }

        private void OnDisable()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            
            Quest.OnQuestsComplete -= OnQuestsComplete;
        }

        private void RegisterButtons()
        {
            CompleteButton.onClick.RemoveAllListeners();
            CompleteButton.onClick.AddListener(() =>
            {
                _next = true;
                _completeAction?.Invoke();
            });
            
            CrossButton.onClick.RemoveAllListeners();
            CrossButton.onClick.AddListener(() =>
            {
                _breakIEnumerator = true;

                GameEventsAssistant.main.StopReadyEvent();
                OnClickButton.Invoke();
            });
        }

        private void UnregisterButtons()
        {
            CompleteButton.onClick.RemoveAllListeners();
            CompleteButton.onClick.AddListener(OnClickButton.Invoke);
        }
        
        private IEnumerator ShowReadyQuestInfo()
        {
            _next = false;
            _breakIEnumerator = false;

            RegisterButtons();
            
            yield return null;
            
            if (selectedLevel)
            {
                yield return ShowQuest(Quest.selected);
                
                GameEventsAssistant.main.StopReadyEvent();
            }
            else
            {
                var readyQuest = GameEventsAssistant.main.GetReadyQuests();
                foreach (var ready in readyQuest)
                {
                    yield return ShowQuest(ready);
                    
                    if (_breakIEnumerator) yield break;
                }
            }

            UnregisterButtons();
        }

        private IEnumerator ShowQuest(Quest quest)
        {
            UpdateInfo(quest);
            
            while (quest.IsReady())
            {
                _completeAction = () =>
                {
                    quest.CompleteIfReady();
                    GameEventsAssistant.main.ResetReadyEvent();
                };
                
                yield return new WaitUntil(() => _next || _breakIEnumerator);

                _next = false;
                
                if (_breakIEnumerator) yield break;
                
                UpdateInfo(quest);
            }
        }

        private void ShakePanel()
        {
            Panel.DOKill();
            Panel.localScale = Vector3.one;
                
            var punch = Panel.DOShakeScale(PunchDuration, PunchStrength);
            punch.Play();
        }
    }

}