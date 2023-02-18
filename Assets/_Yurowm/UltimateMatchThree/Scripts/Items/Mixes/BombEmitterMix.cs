using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yurowm.GameCore;
public class BombEmitterMix : IChipMix
{
    private IEnumerator logic;
    
    public override void Prepare(IChip firstChip, IChip secondChip) {
        var score = 0;
        score += firstChip.destroyable.destroyReward;
        score += secondChip.destroyable.destroyReward;

        Slot target = secondChip.slot;
        IChip bombReference = null;
        if (firstChip && (firstChip is IBomb) && firstChip.original)
        {
            bombReference = firstChip.original.GetComponent<IChip>();
            target = secondChip.slot;
        }
        else if (secondChip && (secondChip is IBomb) && secondChip.original)
        {
            bombReference = secondChip.original.GetComponent<IChip>();
            target = secondChip.slot;
        }
        
        var color = ItemColor.Unknown;
        if (secondChip.colored != null)
            color = secondChip.colored.color;
        
        var context = new HitContext(target, HitReason.Matching);
        logic = EmitBomb(target, bombReference, color, context);
        
        ScoreEffect.ShowScore(target.transform.position, score, color, 2);
    }

    public override IEnumerator Destroying() {
        while (logic.MoveNext())
            yield return logic.Current;
    }

    IEnumerator EmitBomb(Slot slot, IChip bombReference, ItemColor color, HitContext context)
    {
        ISlotContent content = slot.GetCurrentContent();
        if (content && content is IChip) {
            ISlotContent bomb = FieldAssistant.main.Add(bombReference, slot, color);
            bomb.birthDate--;
            yield return new WaitForSeconds(0.3f);
            if (bomb.destroyable != null && !bomb.destroying)
                bomb.slot.HitAndScore(context);
        } else
            slot.HitAndScore(context);
    }
}