using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Yurowm.GameCore;

[RequireComponent (typeof (LayoutGroup))]
public class BoosterPanel : MonoBehaviour {

    [ContentSelector]
    public BoosterButton button;
    public Type type;

    public bool maxThree;
    
    public UnityEvent onEnable;
    public UnityEvent boosterPlaced;

    public enum Type {
        SingleUse,
        MultipleUse
    }

    void OnEnable()
    {
        transform.DestroyChilds();
        
        onEnable.Invoke();
        //
        BoosterInfo.toRelease.Clear();

        List<IBooster> boosterPrefabs = null;
        switch (type)
        {
            case Type.MultipleUse:
                boosterPrefabs = Content.GetPrefabList<IBooster>();
                boosterPrefabs.RemoveAll(x => x is ISingleUseBooster);
                break;
            case Type.SingleUse:
                boosterPrefabs = BoosterAssistant.main.SingleUseBoosters;
                break;
        }

        if (maxThree && boosterPrefabs.Count > 3)
        {
            var list = new List<IBooster>();
            for (var i = 0; i < 3; i++)
            {
                list.Add(boosterPrefabs.Where(x => !list.Contains(x)).GetRandom());
            }
            
            boosterPrefabs = list;        
        }
        
        boosterPrefabs.Sort((x, y) => string.Compare(x.name, y.name, StringComparison.Ordinal));

        foreach (IBooster prefab in boosterPrefabs) {
            if (!Validate(prefab)) continue;
            BoosterButton b = Content.GetItem<BoosterButton>(button.name);
            b.SetPrefab(prefab);
            b.icon.sprite = prefab.icon;
            b.transform.SetParent(transform);
            b.transform.Reset();
        }

        if (boosterPrefabs.Count > 0)
            boosterPlaced.Invoke();
    }

    public static bool Validate(IBooster prefab) {
        if (prefab is ILevelRuleExclusive && !(prefab as ILevelRuleExclusive).IsCompatibleWith(LevelDesign.selected.type))
            return false;
        if (prefab is IGoalExclusive) {
            foreach (var goal in SessionInfo.current.GetGoals())
                if ((prefab as IGoalExclusive).IsCompatibleWithGoal(goal))
                    return true;
            return false;
        }
        return true;
    }
}

public class BoosterInfo
{
    public static List<BoosterInfo> toRelease = new List<BoosterInfo>();
    
    private static BoosterButton _selected;

    public static BoosterButton selected
    {
        set
        {
            if (_selected != value)
            {
                _selected = value;
                refresh?.Invoke();
            }
        }

        get => _selected;
    }

    public static event Action refresh;
    
    public IBooster booster = null;
    public bool boosterSelected = false;
    
    public BoosterInfo(IBooster booster, bool boosterSelected)
    {
        this.booster = booster;
        this.boosterSelected = boosterSelected;
    }
}
