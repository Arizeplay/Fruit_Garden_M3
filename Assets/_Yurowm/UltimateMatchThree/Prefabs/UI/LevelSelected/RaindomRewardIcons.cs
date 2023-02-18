using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yurowm.GameCore;

public class RaindomRewardIcons : MonoBehaviour
{
    public Image Icon;
    public float TimeStep = 0.5f;
    
    private int _index;
    private Coroutine _routine;
    
    private void OnEnable()
    {
        _index = 0;
        _routine = StartCoroutine(Animation());
    }

    private IEnumerator Animation()
    {
        if (AdRandomRewarder.main.Rewards.IsNullOrEmpty())
        {
            yield break;
        }
        
        while (true)
        {
            Icon.sprite = AdRandomRewarder.main.Rewards[_index].icon;
            
            yield return new WaitForSeconds(TimeStep);

            _index++;

            if (_index >= AdRandomRewarder.main.Rewards.Count)
            {
                _index = 0;
            }
        }
    }

    private void OnDisable()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
        }
    }
}
