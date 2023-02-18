using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ninsar.GameEvents.Quests;
using UnityEngine;
using Yurowm.GameCore;

namespace Ninsar.GameEvents
{
    public class GameEventsAssistant : MonoBehaviourAssistant<GameEventsAssistant>
    {
        public List<GameEvent> GameEvents;
        public List<GameEvent> Quests;

        public Transform GameEventParent;
        public Transform QuestParent;

        private Coroutine _routine;
        
        [ContextMenu("ResetStats")]
        public void ResetStats()
        {
            foreach (var gameEvent in GameEvents)
            {
                (gameEvent as Quest)?.ResetStats();
            }

            foreach (var gameEvent in Quests)
            {
                (gameEvent as Quest)?.ResetStats();
            }
        }
        
        private IEnumerator Start()
        {
            DebugPanel.AddDelegate("Add Random Quests", ResetStats);
            
            foreach (var gameEvent in GameEvents)
            {
                gameEvent.Load();
                gameEvent.Init();
                gameEvent.Instantiate(GameEventParent);
            }

            GameEvents.ForEach(e => e.Update());
            
            foreach (var gameEvent in Quests)
            {
                gameEvent.Load();
                gameEvent.Init();
                gameEvent.Instantiate(QuestParent);
            }
            
            Quests.ForEach(e => e.Update());

            yield return new WaitForSeconds(1);
            
            Quest.OnQuestsReady += _ => ResetReadyEvent();
            SessionAssistant.OnLevelEnd += ResetReadyEvent;

            ResetReadyEvent();
        }
        
        public void ResetReadyEvent()
        {
            StopReadyEvent();
            
            _routine = StartCoroutine(ShowPages());
        }

        public void StopReadyEvent()
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
            }
        }

        private IEnumerator ShowPages()
        {
            yield return null;

            var readyQuests = GetReadyQuests();
            while (readyQuests.Count > 0)
            {
                var quest = readyQuests.First();

                readyQuests.Remove(quest);
                
                yield return new WaitWhile(() => FieldAssistant.main.sceneFolder != null);
                
                yield return new WaitWhile(() => UIAssistant.main.GetCurrentPage().HasTag("quest"));
                yield return new WaitWhile(() => UIAssistant.main.GetCurrentPage().HasTag("question"));
                yield return new WaitWhile(() => UIAssistant.main.GetCurrentPage().HasTag("game"));
                
                yield return new WaitForSeconds(.5f);
                
                if (quest.ShowPageIfReady && quest.IsReady())
                {
                    Quest.selected = quest;
                
                    UIAssistant.main.ShowPage(!quest.GetPage().IsNullOrEmpty() ? quest.GetPage() : "QuestComplete");
                }
            }
        }
        
        public List<Quest> GetReadyQuests()
        {
            var quests = new List<GameEvent>();
            quests.AddRange(Quests);
            quests.AddRange(GameEvents);
            return quests.Select(x => x as Quest)
                .Where(x => x is { } quest && quest.IsReady())
                .ToList();
        }
    }
}
