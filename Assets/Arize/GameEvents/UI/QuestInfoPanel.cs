using System.Collections;
using Ninsar.GameEvents.Quests;
using UnityEngine;

namespace Ninsar.GameEvents.UI
{
    public class QuestInfoPanel : QuestInfo
    {
        private Coroutine _coroutine;

        private void OnEnable()
        {
            _coroutine = StartCoroutine(ShowInfo());
        }
        
        private void OnDisable()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
        }
        
        private IEnumerator ShowInfo()
        {
            yield return null;
            
            UpdateInfo(Quest.selected);
        }
    }
}