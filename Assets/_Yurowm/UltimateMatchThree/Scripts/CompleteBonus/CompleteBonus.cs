using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yurowm.GameCore;

public abstract class CompleteBonus : ILiveContent {
    
    public AnimationCurve averageScorePerMove = AnimationCurve.Linear(0, 3, 1, 10);
    
    internal abstract IEnumerator Logic();

    internal abstract bool IsComplete();
    
    public override void OnKill()
    {
        while (SessionInfo.current.BurnMove()) 
        {
            var random = UnityEngine.Random.Range(0f, 1f);
            var addScore = (int) averageScorePerMove.Evaluate(random);

            SessionInfo.current.AddScorePoint(addScore);
        }
    }
}