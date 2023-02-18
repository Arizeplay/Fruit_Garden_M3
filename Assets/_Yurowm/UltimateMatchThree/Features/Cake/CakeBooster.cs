using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Yurowm.GameCore;

namespace _Yurowm.UltimateMatchThree.Features.Cake
{
    public class CakeBooster : IMultipleUseBooster
    {
        public float maxDistance;
        [FormerlySerializedAs("СookingEffect")]
        public CookingEffect cookingEffect;
        
        public override string FirstMessage() {
            return LocalizationAssistant.main[FirstMessageLocalizedKey()];
        }

        public override bool ShowButton => true;
        public override bool ShowExitButton => false;

        private Dictionary<Slot, IChip> _field;

        public override IEnumerator Logic() {
            if (!IsCanceled())
            {
                _field = new Dictionary<Slot, IChip>();
                
                foreach (var s in Slot.allActive)
                {
                    if (s.Value && s.Value.chip && !s.Value.block)
                    {
                        var chip = s.Value.chip.original.GetComponent<IChip>();
                        if (chip && !(chip is IngredientChip))
                        {
                            _field.Add(s.Value, chip);
                        }
                    }
                }
                
                List<Slot> targets = new List<Slot>(Slot.allActive.Values);

                foreach (var slot in targets)
                {
                    if (slot)
                    {
                        slot.HitAndScore();
                        
                        if (slot.chip is IBomb)
                        {
                            slot.chip.Hide();
                        }
                        
                        yield return null;
                    }
                }

                yield return new WaitForSeconds(0.1f);
                
                foreach (var sChip in _field)
                {
                    FieldAssistant.main.CreateNewContent(sChip.Value, sChip.Key, Vector3.zero);
                }

                var powerUp = new List<IBomb>();
                List<Slot> slots = new List<Slot>();
                List<Slot> allActive = new List<Slot>(Slot.allActive.Values);
                allActive.RemoveAll(x => x.block || !x.chip || !(x.chip is IDefaultSlotContent));
                allActive.RemoveAll(x => x.slot.GetClosestSlots().Contains(null));
                List<Slot> targetsWithoutBorder = new List<Slot>(allActive);
                
                var count = 4;
                while (count > 0) {
                    count--;
                    
                    Slot slot = null;

                    var distTargets = allActive.OrderBy(x =>
                    {
                        return slots.Aggregate(float.MaxValue, (current, s) => Mathf.Min(current, x.position.EightSideDistanceTo(s.position)));
                    }).Reverse().ToList();

                    if (distTargets.Count > 0)
                    {
                        slot = distTargets.GetRange(0, Mathf.Clamp(distTargets.Count, 1, 3)).GetRandom();
                        slots.Add(slot);
                        allActive.Remove(slot);
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

                    var prefab = Content.GetPrefab<SimpleBomb>();
                    var slotContext = FieldAssistant.main.Add(prefab, slot);
                    powerUp.Add((IBomb)slotContext);
                    yield return new WaitForSeconds(0.1f);
                }
                
                // List<Slot> closest = targets.Where(x =>
                // {
                //     var dist = Vector2.Distance(x.transform.position.To2D(), GameCamera.main.transform.position.To2D() * Vector2.right);
                //     Debug.Log(dist);
                //     return dist < maxDistance;
                // }).ToList();
                // targets.RemoveAll(x => closest.Contains(x));
                // foreach (var c in closest)
                // {
                //     if(c.chip)
                //         c.chip.Hide();
                // }
                //
                // CookingEffect cookingEffect = Content.Emit(this.cookingEffect);
                // cookingEffect.transform.position = GameCamera.main.transform.position.To2D() * Vector2.right;
                // cookingEffect.SetTargets(chips.ToArray());
                // cookingEffect.Play();
                //
                // yield return new WaitWhile(cookingEffect.IsComplete);

                SessionInfo.current.rule.matchDate++;
                
                yield return new WaitForSeconds(.1f);
            }

            BoosterAssistant.main.boosterMode = null;
        }
        
        public override IEnumerator LogicOnGameStart()
        {
            yield return Logic();
        }
    }
}