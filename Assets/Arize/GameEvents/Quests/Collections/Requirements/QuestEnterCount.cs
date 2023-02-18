using System;
using UnityEngine;
namespace Ninsar.GameEvents.Quests.Collections.Requirements
{

    [Serializable]
    public class QuestEnterCount : QuestRequirement
    {
        public override void RegisterEvents()
        {
            TrueTime.onGetTime += time =>
            {
                string lastDate = null;
                if (PlayerPrefs.HasKey("QuestEnterCount_Time"))
                {
                    lastDate = PlayerPrefs.GetString("QuestEnterCount_Time");
                }
                if (lastDate != null)
                {
                    var lastDateTime = DateTime.Parse(lastDate);
                    var diff = time - lastDateTime;
                    if (diff.TotalDays >= 1)
                    {
                        PlayerPrefs.SetString("QuestEnterCount_Time", time.ToShortDateString());
                        OnItemAdd(1);
                    }
                }
                else
                {
                    PlayerPrefs.SetString("QuestEnterCount_Time", time.ToShortDateString());
                }
            };
        }
        
        public override void UnregisterEvents()
        {
            
        }
    }
}