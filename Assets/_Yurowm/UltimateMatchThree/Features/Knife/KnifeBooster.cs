using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yurowm.GameCore;

namespace _Yurowm.UltimateMatchThree.Features.Firework.Scripts
{
    public class KnifeBooster : IMultipleUseBooster
    {
        public override bool ShowButton => true;
    
        Slot slot;
    
        public override string FirstMessage() {
            return LocalizationAssistant.main[FirstMessageLocalizedKey()];
        }
    
        public override void Initialize() {
            slot = null;
            base.Initialize();
        }

        void OnClick(Slot slot) {
            this.slot = slot;
        }
    
        public override IEnumerator Logic() {
            List<Slot> targets = Slot.allActive.Values.Where(x => x.color.IsPhysicalColor() && x.chip && x.chip is IDefaultSlotContent).ToList();
            targets.ForEach(x => x.Highlight());

            if (targets.Count == 0)
            {
                Cancel();
                
                BoosterAssistant.main.boosterMode = null;
                
                yield break;
            }

            ControlAssistant.main.ChangeMode(ControlAssistant.ControlMode.Click);
            ControlAssistant.main.onClick += OnClick;

            while ((!slot || !targets.Contains(slot)) && !IsCanceled())
                yield return new WaitForSeconds(0.1f);

            ControlAssistant.main.ChangeMode(ControlAssistant.ControlMode.Regular);
            ControlAssistant.main.onClick -= OnClick;
            targets.ForEach(x => x.Unlight());

            if (!IsCanceled())
            {
                yield return new WaitForSeconds(0.1f);
                var prefab = Content.GetPrefabList<CrossBomb>();
                var color = slot.color;
                var crossBomb = prefab.Where(x => (x.right && x.left) != (x.top && x.bottom)).GetRandom();
                var slotContent =  FieldAssistant.main.Add(crossBomb, slot);
                slotContent.slot.Repaint(color);
            
                SessionInfo.current.rule.matchDate++;
            }

            BoosterAssistant.main.boosterMode = null;
        }
    
        public override IEnumerator LogicOnGameStart()
        {
            yield return Logic();
        }
    }
}