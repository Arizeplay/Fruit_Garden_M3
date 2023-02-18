using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yurowm.GameCore;


public class BombBooster : ISingleUseBooster {

    public override string FirstMessage() {
        return LocalizationAssistant.main[FirstMessageLocalizedKey()];
    }

    public override bool ShowButton => false;

    public override IEnumerator Logic() {
        var prefab = Content.GetPrefab<SimpleBomb>();
        var colors = SessionInfo.current.colorMask.Values.ToList();
        var count = 1;
        while (count > 0) {
            count--;
            var slotContent = FieldAssistant.main.Add(prefab);
            if (slotContent)
            {
                slotContent.slot.Repaint(colors.GetRandom());
            }
            yield return new WaitForSeconds(0.1f);
        }
            
        yield return new WaitForSeconds(0.1f);
    }
    
    public override IEnumerator LogicOnGameStart()
    {
        return Logic();
    }
}