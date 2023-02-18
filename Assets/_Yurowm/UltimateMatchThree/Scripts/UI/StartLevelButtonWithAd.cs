using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Yurowm.GameCore;

public class StartLevelButtonWithAd : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _opens;
    private Button _button;
    private static List<IBooster> _boosters;
    public static IBooster[] BoostersSelected
    {
        get
        {
            if (_boosters == null)
            {
                _boosters = new List<IBooster>();
                
                if (PlayerPrefs.HasKey("Boosters_From_Ads"))
                {
                    var json = PlayerPrefs.GetString("Boosters_From_Ads");
                    var data = JsonConvert.DeserializeObject<List<int>>(json);

                    Debug.Log(data.Count);

                    foreach (var itemID in data)
                    {
                        _boosters.Add(Content.GetPrefabList<ISingleUseBooster>().First(x => x.itemID == (ItemID)itemID));   
                    }
                }
            }
            
            return _boosters.ToArray();
        }
    }

    private void Awake()
    {
        _button = GetComponent<Button>();
        _boosters = BoostersSelected.ToList();
        
        ItemCounter.refresh += Refresh;
        SessionAssistant.OnRunLevelDesign += OnRunLevelDesign;
        
        Refresh();
    }
    
    private void OnRunLevelDesign(LevelDesign levelDesign)
    {
        foreach (var booster in _boosters)
        {
            CurrentUser.main[booster.itemID] += 1;
            BoosterInfo.toRelease.Add(new BoosterInfo(booster, true));
        }
        
        PlayerPrefs.DeleteKey("Boosters_From_Ads");
        
        _boosters.Clear();
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void OnClick()
    {
        if (CPanel.uiAnimation > 0) return;
        
        Advertising.main.ShowAds(AdType.Rewarded, GetBooster);
    }

    private void Refresh()
    {
        for (var i = 0; i < _opens.Count; i++)
        {
            _opens[i].SetActive(i < _boosters.Count);
        }
        
        _button.interactable = Advertising.main.CountOfReadyAds(AdType.Rewarded) > 0 && _boosters.Count < 3;
    }

    private void GetBooster()
    {
        var reward = AdRandomRewarder.main.GetReward(_boosters.Select(x => x.itemID).ToList());
        if (reward == null)
        {
            return;
        }
        
        _boosters.Add(reward.booster);

        var list = _boosters.Select(booster => (int)booster.itemID).ToList();
        var data = JsonConvert.SerializeObject(list);

        Debug.Log(data);
        
        PlayerPrefs.SetString("Boosters_From_Ads", data);
        PlayerPrefs.Save();
        
        Refresh();
        
        ItemCounter.RefreshAll();
    }
}