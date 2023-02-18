using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using _Yurowm.UltimateMatchThree.Features.Multiplyer;
using UnityEngine;
using Yurowm.GameCore;

public class Rainbow : IChip, IBomb, IMixable {
    public int mixingPriority;

    [ContentSelector]
    public ThunderEffect thunderEffect;

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

    protected ItemColor targetColor = ItemColor.Unknown;
    public IEnumerator Mixing(IChip secondChip) {   
        sound.Play("Destroying");
        animator.Play("Destroying");

        targetColor = colored == null ? ItemColor.Unknown : colored.color;

        if (!targetColor.IsPhysicalColor() && secondChip && secondChip.colored != null)
            targetColor = secondChip.colored.color;
        
        if (!targetColor.IsPhysicalColor())
            targetColor = SessionInfo.current.colorMask.Values.GetRandom();

        if (secondChip && destroyingEffect) {
            IEffect effect = Content.Emit(destroyingEffect);
            effect.transform.position = transform.position;

            if (targetColor.IsPhysicalColor())
                effect.Repaint(targetColor);

            effect.Play();
        }

        IChip bombReference = null;
        if (secondChip && (secondChip is IBomb) && !(secondChip is Rainbow) && secondChip.original)
            bombReference = secondChip.original.GetComponent<IChip>();
        
        List<Slot> targets = GetTargets(secondChip);

        if (secondChip) targets.Remove(secondChip.slot);
        targets.Remove(slot);

        HitContext context = new HitContext(targets, HitReason.Matching);
        
        ThunderEffect thunder = Content.Emit(thunderEffect);
        thunder.transform.position = transform.position;
        if (targetColor.IsPhysicalColor())
            thunder.Repaint(targetColor);
        thunder.SetTargets(targets.ToArray());
        thunder.Play();
        
        
        if (bombReference)
            thunder.onReachCoroutine = targets.Select(t => EmitBomb(t, bombReference, targetColor, context)).ToArray();
        else
        {
            foreach (Slot target in targets) 
            {
                thunder.onReach += () => target.HitAndScore(context);
            }
        }


        yield break;
    }

    public virtual List<Slot> GetTargets(IChip secondChip) {
        if (secondChip is Rainbow)
            return Slot.allActive.Values.Where(x => x.GetCurrentContent() is IDestroyable).ToList();
        else
            return Slot.allActive.Values.Where(x => x.color == targetColor).ToList();
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

    public override void Serialize(XElement xContent) {}

    public override void Deserialize(XElement xContent, SlotContent slotContent) {}

    public int GetMixingLogicPriority() {
        return mixingPriority;
    }
}
