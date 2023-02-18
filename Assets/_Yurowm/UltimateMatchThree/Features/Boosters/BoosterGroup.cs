using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yurowm.GameCore;

public class BoosterGroup : MonoBehaviour
{
    public Transform parent;
    [ContentSelector]
    public BoosterButton button;
    public BoosterPanel.Type type;

    public enum Type {
        SingleUse,
        MultipleUse
    }

    void OnEnable() {
        var boosterPrefabs = Content.GetPrefabList<IBooster>();
        if (type == BoosterPanel.Type.MultipleUse)
            boosterPrefabs.RemoveAll(x => x is ISingleUseBooster);
        else
            boosterPrefabs.RemoveAll(x => x is IMultipleUseBooster);
        
        boosterPrefabs.Sort((x, y) => string.Compare(x.name, y.name, StringComparison.Ordinal));

        parent.DestroyChilds();
        
        foreach (IBooster prefab in boosterPrefabs) {
            if (!Validate(prefab)) continue;

            BoosterButton b = Content.GetItem<BoosterButton>(button.name);
            b.SetPrefab(prefab);
            b.icon.sprite = prefab.icon;
            b.transform.SetParent(parent);
            b.transform.Reset();
        }
    }

    public static bool Validate(IBooster prefab) {
        if (prefab is ILevelRuleExclusive && !(prefab as ILevelRuleExclusive).IsCompatibleWith(SessionInfo.current.rule))
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
