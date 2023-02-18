using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace _Yurowm.UltimateMatchThree.Features.Cake
{
    public class CookingEffect : IEffect, ISounded, IAnimated
    {
        public float Speed;
        public float DeathTime;
        public Transform Target;

        //public ChipCookingEffect 
        
        private IChip[] _chips;
        private bool _isComplete;
        public override bool IsComplete()
        {
            return _isComplete;
        }
        
        public void SetTargets(IChip[] chips)
        {
            _chips = chips;
        }
        
        public override void Launch()
        {
            StartCoroutine(Logic());
        }

        private IEnumerator Logic()
        {
            if (sound) sound.Play("OnAwake");
            if (animator) animator.Play("OnAwake");

            while (animator && animator.IsPlaying())
                yield return 0;

            var chips = _chips.ToList();
            while (chips.Count > 0)
            {
                for (var i = 0; i < chips.Count; i++)
                {
                    var chip = chips[i];
                    if (chip)
                    {
                        chip.transform.position = GetNewPosition(chip.transform.position, Time.deltaTime * Speed);
                        if (Vector3.Distance(chip.transform.position, Target.position) < 0.1f)
                        {
                            Project.onChipCrush.Invoke();
                            Project.onSlotContentPrepareToDestroy.Invoke(chip);
                            Project.onSlotContentDestroyed.Invoke(chip);
                            Project.onSomeContentDestroyed.Invoke();
                            
                            chip.Hide();
                            chips.Remove(chip);
                        }
                    }
                    else
                    {
                        chips.Remove(chip);
                    }
                }

                Debug.Log(chips.Count);
                
                yield return null;
            }

            if (sound) sound.Play("OnDeath");
            if (animator) animator.Play("OnDeath");
            
            yield return StartCoroutine(Death());

            while (animator && animator.IsPlaying())
                yield return 0;
            
            _isComplete = true;
        }
        
        public virtual IEnumerator Death() {
            yield return new WaitForSeconds(DeathTime);
        }
        
        public IEnumerator GetSoundNames()
        {
            yield return "OnAwake";
            yield return "OnDeath";
        }
        public IEnumerator GetAnimationNames()
        {
            yield return "OnAwake";
            yield return "OnDeath";
        }
        
        public Vector3 GetNewPosition(Vector3 startPosition, float time)
        {
            return Vector3.MoveTowards(startPosition, Target.position, time);
        }
    }
}