using System.Collections;
using System.Xml.Linq;
using Yurowm.GameCore;
namespace _Yurowm.UltimateMatchThree.Features.Pinapple
{
    public class Pineapple : IChip, IBomb, INeedToBeSetup {
        
        public int destroyReward => 60;
    
        public IEnumerator Destroying() {
            Explode(transform.position, 3, 100);
    
            int2 position = slot.position;
    
            sound.Play("Destroying");
            animator.Play("Destroying");
    
            var hitGroup = new System.Collections.Generic.List<Slot> { slot };
            foreach (Side side in Utils.allSides) {
                int2 coord = position + side.ToInt2();
                if (Slot.allActive.ContainsKey(coord))
                    hitGroup.Add(Slot.allActive[coord]);
            }
            HitContext context = new HitContext(hitGroup, HitReason.BombExplosion);
            int score = destroyReward;
            hitGroup.ForEach(s => score += s.Hit(context));
            ScoreEffect.ShowScore(transform.position, score, ItemColor.Universal, 2f);
    
            while (animator.IsPlaying("Destroying"))
                yield return 0;
        }
    
        public void Explode() {
            slot.Hit();
        }
    
        public void OnSetup(Slot slot) { }
        public void OnSetupByContentInfo(Slot slot, SlotContent info) { }
        public override void Serialize(XElement xContent) { }
        public override void Deserialize(XElement xContent, SlotContent slotContent) { }
    }
}