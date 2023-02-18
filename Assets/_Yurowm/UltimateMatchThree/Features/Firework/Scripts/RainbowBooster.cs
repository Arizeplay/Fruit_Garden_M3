using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yurowm.GameCore;

class RainbowBooster : ISingleUseBooster {
    [ContentSelector]
    public ThunderEffect thunderEffect;

    public override bool ShowButton => true;
    
    Slot slot;
    
    public override string FirstMessage() {
        return LocalizationAssistant.main[FirstMessageLocalizedKey()];
    }
    
    public override void Initialize() {
        slot = null;
        base.Initialize();
    }

    public override IEnumerator Logic() {
        var prefab = Content.GetPrefab<Rainbow>();
        var count = 1;
        while (count > 0) {
            count--;
            FieldAssistant.main.Add(prefab);
            yield return new WaitForSeconds(0.1f);
        }
            
        yield return new WaitForSeconds(0.1f);
    }
    
    public override IEnumerator LogicOnGameStart()
    {
        yield return Logic();
    }
}