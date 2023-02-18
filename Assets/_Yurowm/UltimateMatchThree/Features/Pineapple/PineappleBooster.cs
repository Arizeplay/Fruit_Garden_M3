using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Yurowm.GameCore;
namespace _Yurowm.UltimateMatchThree.Features.Pinapple
{
    public class PineappleBooster : IMultipleUseBooster
    {
        public override string FirstMessage() {
            return LocalizationAssistant.main[FirstMessageLocalizedKey()];
        }

        public override bool ShowButton => true;
        public override bool ShowExitButton => false;

        public override IEnumerator Logic() {
            if (!IsCanceled())
            {
                List<Slot> slots = new List<Slot>();
                List<Slot> targets = new List<Slot>(Slot.allActive.Values);
                targets.RemoveAll(x => x.GetCurrentContent() != null && (x.block || !x.chip || !(x.chip is IDefaultSlotContent)));
                List<Slot> targetsWithoutBorder = new List<Slot>(targets);
                targets.RemoveAll(x => x.slot.GetClosestSlots().Contains(null));
                targets.RemoveAll(x => x.slot.GetClosestSlots().Contains(slot => slot.block));

                const int maxCount = 3;
                var count = maxCount;
                while (count > 0) {
                    count--;
                    
                    Slot slot = null;

                    var distTargets = targets.OrderBy(x =>
                    {
                        return slots.Aggregate(float.MaxValue, (current, s) => Mathf.Min(current, x.position.EightSideDistanceTo(s.position)));
                    }).Reverse().ToList();

                    if (distTargets.Count > 0)
                    {
                        slot = distTargets.GetRange(0, Mathf.Clamp(distTargets.Count, 1, 3)).GetRandom();
                        slots.Add(slot);
        
                        targets.Remove(slot);
                    }

                    if (slot == null)
                    {
                        if (targetsWithoutBorder.Count > 0)
                        {
                            slot = targetsWithoutBorder.Where(x => !slots.Contains(x)).GetRandom();
                            slots.Add(slot);
                            targetsWithoutBorder.Remove(slot);
                        }
                    }
                }

                slots.RemoveAll(x => x == null);
                slots = slots.Distinct().ToList();

                if (slots.Count < maxCount)
                {
                    Cancel();
                    
                    BoosterAssistant.main.boosterMode = null;
                    
                    yield break;
                }
                
                yield return new WaitForSeconds(0.1f);

                var powerUp = new List<IBomb>();
                for (int i = 0; i < maxCount; i++)
                {
                    var slot = slots[i];
                    var prefab = Content.GetPrefab<Pineapple>();
                    var slotContext = slot == null ? FieldAssistant.main.Add(prefab) : FieldAssistant.main.Add(prefab, slot);
                    
                    Debug.Log(slotContext);

                    powerUp.Add((IBomb)slotContext);

                    yield return new WaitForSeconds(0.1f);
                }

                yield return new WaitForSeconds(0.1f);

                SessionInfo.current.rule.matchDate++;
                
                yield return CollapsePowerups(powerUp);
            }

            BoosterAssistant.main.boosterMode = null;
        }
        
        public override IEnumerator LogicOnGameStart()
        {
            yield return Logic();
        }
        
        internal IEnumerator CollapsePowerups(List<IBomb> powerUp) {
            while (powerUp.Count > 0) {
                powerUp = powerUp.Where(x => !x.destroying).ToList();
                if (powerUp.Count > 0)
                    powerUp.GetRandom().Explode();
                yield return new WaitForSeconds(0.2f);
            }

            yield return new WaitForSeconds(0.2f);
        }
    }
}