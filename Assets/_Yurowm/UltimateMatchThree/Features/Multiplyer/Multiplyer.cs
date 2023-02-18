using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Yurowm.GameCore;

namespace _Yurowm.UltimateMatchThree.Features.Multiplyer
{
    public class Multiplyer : IChip, IBomb, IMixable, IColored, INeedToBeSetup
    {
        private static List<Slot> globalTargets;

        public int mixingPriority;

        [FormerlySerializedAs("outEffect")]
        [ContentSelector]
        public StrikeEffect OutEffect;
        
        [FormerlySerializedAs("inEffect")]
        [ContentSelector]
        public StrikeEffect InEffect;
        
        [FormerlySerializedAs("TargetPos")]
        public float EnterHeight = 5;
        public float ExitHeight = 5;

        public float Offset = 0.5f;
        
        ItemColor _color;
        public ItemColor color {
            get {
                return _color;
            }

            set {
                _color = value;
            }
        }
        
        public int GetMixingLogicPriority()
        {
            return mixingPriority;
        }
        
        public int destroyReward => 60;

        public IEnumerator Destroying() {
            var mixing = Mixing(null);
            while (mixing.MoveNext())
                yield return mixing.Current;

            while (animator.IsPlaying())
                yield return 0;
        }

        public void Explode() {
            slot.HitAndScore();
        }
        
        public IEnumerator Mixing(IChip secondChip) {   
            sound.Play("Destroying");
            animator.Play("Destroying");

            var horizontalExtent = GameCamera.camera.orthographicSize * Screen.width / Screen.height;

            IChip bombReference = null;
            if (secondChip && (secondChip is IBomb) && secondChip.original)
                bombReference = secondChip.original.GetComponent<IChip>();

            var secondChipColor = ItemColor.Unknown;
            if (secondChip && secondChip.colored != null)
            {
                secondChipColor = secondChip.colored.color;
            }
            
            var targetPos = transform.position;
            
            StrikeEffect outEffect = Content.Emit(OutEffect);
            outEffect.transform.position = targetPos;
            outEffect.SetTarget(new Vector2(horizontalExtent + Offset, ExitHeight) + (Vector2)GameCamera.camera.transform.position);
            if (colored.color.IsPhysicalColor())
                outEffect.Repaint(colored.color);
            outEffect.Play();

            if (destroyingEffect) {
                IEffect effect = Content.Emit(destroyingEffect);
                effect.transform.position = targetPos;
                if (secondChipColor.IsPhysicalColor())
                    effect.Repaint(secondChipColor);
                effect.Play();
            }
            
            while (!outEffect.IsComplete())
            {
                yield return null;
            }

            var chips = new List<Slot>();
            var colors = new List<ItemColor>();
            foreach (var goal in SessionInfo.current.GetGoals())
            {
                if (goal is ColorCollectionGoal colorCollectionGoal)
                {
                    colors = colorCollectionGoal.colorTarget
                        .Where(t => t.Value > 0 && Slot.allActive.Values.Contains(x => x.color == t.Key))
                        .ToDictionary().Keys
                        .ToList();
                
                    if (colors.Count == 0)
                        colors = SessionInfo.current.colorMask.Values.ToList();
                
                    if (colors.Count == 0) yield break;
                }
                else
                {
                    colors = SessionInfo.current.colorMask.Values.ToList();

                    if (colors.Count == 0)
                        colors = new List<ItemColor> { ItemColor.Unknown };
                }

                if (goal is CollectionGoal collectionGoal)
                {
                    if (collectionGoal.target is Glass)
                    {
                        chips.AddRange(GetGlassTargets());
                    }
                    else if (collectionGoal.target is Sandwich)
                    {
                        chips.AddRange(GetSandwichTargets());
                    }
                }
            }

            if (chips.Count < 3)
            {
                chips.AddRange(GetTargets());
            }
            
            var targets = new List<Slot>();
            for (var i = 0; i < 3; i++)
            {
                chips = chips.Where(x => !globalTargets.Contains(x) && !targets.Contains(x)).ToList();
                
                if (chips.Count <= 0) continue;
                
                var target = chips.GetRandom();
                targets.Add(target);
                globalTargets.Add(target);
            }

            var indexColor = 0;
            var hitContext = new HitContext(targets, HitReason.Matching);
            foreach (Slot target in targets) {
                StrikeEffect effect = Content.Emit(InEffect);
                effect.transform.position = new Vector2((horizontalExtent + Offset) * -1, EnterHeight) + (Vector2)GameCamera.camera.transform.position;

                var randomColor = colors[indexColor];
                indexColor++;
                if (indexColor > colors.Count - 1)
                {
                    indexColor = 0;
                }
                
                if (randomColor.IsPhysicalColor())
                    effect.Repaint(randomColor);
                effect.SetTarget(target.transform);
                effect.Play();
            
                Slot slot = target;
                if (bombReference)
                    effect.onReachCoroutine = EmitBomb(slot, bombReference, target.color, hitContext);
                else
                    effect.onReachCoroutine = ChangeColorAndHit(slot, randomColor, hitContext);
            }

            yield break;
        }

        public IEnumerator ChangeColorAndHit(Slot slot, ItemColor color, HitContext context)
        {
            if(slot.chip)
                slot.chip.AddImpuls(new Vector3(0,1,0));
            
            yield return new WaitForSeconds(.2f);
            
            if (slot)
            {
                slot.Repaint(color);
                yield return new WaitForSeconds(.5f);
                slot.HitAndScore(context);
            }
        }
        public virtual List<Slot> GetTargets()
        {
            return Slot.allActive.Values
                .Where(x =>
                {
                    var ctx = x.GetCurrentContent();
                    return x.chip != null && ctx != null && ctx is IDefaultSlotContent;
                }).ToList();
        }
        
        public virtual List<Slot> GetGlassTargets()
        {
            return Slot.allActive.Values
                .Where(x =>
                {
                    var ctx = x.GetCurrentContent();
                    return x.chip != null && ctx != null && x.slot.Content().Any(c => c is Glass);
                }).ToList();
        }
        
        public virtual List<Slot> GetSandwichTargets()
        {
            return Slot.allActive.Values
                .Where(x =>
                {
                    var ctx = x.GetCurrentContent();
                    var isSandwichNear = x.GetClosestSlots().Any(s => s && s.block && s.block is Sandwich);
                        
                    return x.chip != null && ctx != null && isSandwichNear;
                }).ToList();
        }

        IEnumerator EmitBomb(Slot slot, IChip bombReference, ItemColor color, HitContext context) {
            ISlotContent content = slot.GetCurrentContent();
            if (content && content is IChip && content is IDefaultSlotContent) {
                ISlotContent bomb = FieldAssistant.main.Add(bombReference, slot, color);
                bomb.birthDate--;
                yield return new WaitForSeconds(0.3f);
                if (bomb.destroyable != null && !bomb.destroying)
                    bomb.slot.HitAndScore(context);
            } else
                slot.HitAndScore(context);
        }
        
        public void OnSetup(Slot slot)
        {
            globalTargets = new List<Slot>();
            Repaint(this, color.IsPhysicalColor() ? color : SessionInfo.current.colorMask.Values.GetRandom());
        }

        public void OnSetupByContentInfo(Slot slot, SlotContent info) 
        {
            globalTargets = new List<Slot>();
            color = info["color"].ItemColor;
            Repaint(this, color);
        }
        
        public override void Serialize(XElement xContent) {
            xContent.Add(new XAttribute("color", (int) _color));
        }

        public override void Deserialize(XElement xContent, SlotContent slotContent) {
            slotContent["color"].ItemColor = (ItemColor) int.Parse(xContent.Attribute("color").Value);
        }
    }
}