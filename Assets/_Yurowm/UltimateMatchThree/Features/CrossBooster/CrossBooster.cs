using System.Collections;
using System.Linq;
using UnityEngine;
using Yurowm.GameCore;
using UnityEngine;

namespace _Yurowm.UltimateMatchThree.Features.CrossBooster
{
    public class CrossBooster : ISingleUseBooster {

        public override string FirstMessage() {
            return LocalizationAssistant.main[FirstMessageLocalizedKey()];
        }

        public override bool ShowButton => false;

        public override IEnumerator Logic() {
            var prefab = Content.GetPrefabList<CrossBomb>();
            var crossBomb = prefab.Where(x => (x.right && x.left) != (x.top && x.bottom)).GetRandom();
            var colors = SessionInfo.current.colorMask.Values.ToList();
            var count = 1;
            while (count > 0) {
                count--;
                var slotContent = FieldAssistant.main.Add(crossBomb);
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
}