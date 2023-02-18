using System;
using System.Collections;
using UnityEngine;
namespace _Yurowm.UltimateMatchThree.Features.Cake
{
    public class ChipCookingEffect : IEffect
    {
        public float Speed = 5;

        private Vector2 _position;
        private bool _isComplete;
        
        public void SetTargetPosition(Vector3 pos)
        {
            _position = pos;
        }
        
        public override bool IsComplete()
        {
            return _isComplete;
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

            while (Vector3.Distance(transform.position, _position) < 0.1f)
            {
                transform.position = GetNewPosition(transform.position, Time.deltaTime * Speed);
                
                yield return null;
            }
            
            if (sound) sound.Play("OnDeath");
            if (animator) animator.Play("OnDeath");
            
            while (animator && animator.IsPlaying())
                yield return 0;

            _isComplete = true;
        }
        
                
        public Vector3 GetNewPosition(Vector3 startPosition, float time)
        {
            return Vector3.Lerp(startPosition, _position, time);
        }
    }
}