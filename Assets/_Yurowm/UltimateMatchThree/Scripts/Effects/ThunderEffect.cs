using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yurowm.GameCore;
using Random = UnityEngine.Random;

public class ThunderEffect : IEffect, ISounded, IAnimated {
    bool isComplete = false;
    
    public FloatRange speed;
    public float time = 1;
    public float waitTime = 1;
    public float spawnTime = 0.2f;
    public float maxSpawnTime = 0.05f;
    public float radius = 0.2f;

    public AnimationCurve move;
    
    public GameObject highlighter;
    public Vector3 offset;

    public Action onReach = delegate {};
    public IEnumerator[] onReachCoroutine = null;

    public bool gravityLocker = true;
    public GameObject TrailPrefab;

    private List<GameObject> _highlighters;
    private List<TrailTarget> _trails;
    private float _speed;
    
    public class TrailTarget
    {
        public GameObject Trail;
        public Slot Target;
        public float Speed;
        public AnimationCurve MoveCurve;

        public float time;

        public TrailTarget(GameObject trail, Slot target, float speed, AnimationCurve moveCurve)
        {
            Trail = trail;
            Target = target;
            Speed = speed;
            MoveCurve = moveCurve;
        }

        public void Move(float deltaTime)
        {
            time += deltaTime;

            Trail.transform.position = GetNewPosition(Trail.transform.position, Target.transform.position, MoveCurve.Evaluate(time) * Speed * deltaTime);
        }
            
        public Vector3 GetNewPosition(Vector3 startPosition, Vector3 targetPosition, float speed)
        {
            return Vector3.MoveTowards(startPosition, targetPosition, speed);
        }
        
        public bool IsComplete()
        {
            return Trail.transform.position == Target.transform.position;
        }
    }
    
    public override bool IsComplete() {
        return isComplete;
    }

    protected Slot[] targets = null;
    Vector3[] targetPositions = null;

    public void SetTargets(Slot[] targets) {
        this.targets = targets;
    }

    public override void Launch()
    {
        StartCoroutine(InitTiles());
        StartCoroutine(Logic());
    }

    private IEnumerator InitTiles()
    {
        if (targets != null)
        {
            targetPositions = new Vector3[targets.Length];
            for (int i = 0; i < targetPositions.Length; i++)
            {
                targetPositions[i] = targets[i].transform.position;
            }
        }
        
        var delay = spawnTime / targets.Length;
        if (delay > maxSpawnTime)
        {
            delay = maxSpawnTime;
        }
        
        _trails = new List<TrailTarget>();

        foreach (var target in targets)
        {
            var trail = Instantiate(TrailPrefab, transform);
            var dir = (target.transform.position - transform.position).normalized;
            
            trail.transform.position = transform.position + dir * radius;
            _speed = Random.Range(speed.min, speed.max);
            _trails.Add(new TrailTarget(trail, target, _speed, move));
            yield return new WaitForSeconds(delay);
        }
    }
    
    IEnumerator Logic() {
        if (gravityLocker) ISlotContent.gravityLockers.Add(this);
        if (sound) sound.Play("OnAwake");
        if (animator) animator.Play("OnAwake");

        Transform subEffect = transform.Find("OnAwake");
        if (subEffect) {
            subEffect.SetParent(transform.parent);
            subEffect.gameObject.SetActive(true);
        }

        _highlighters = new List<GameObject>();
        
        while (true)
        {
            foreach (var trail in _trails)
            {
                if (!trail.IsComplete())
                {
                    trail.Move(Time.deltaTime);

                    if (trail.IsComplete())
                    {
                        trail.Target.OnPress();

                        var effect = Instantiate(highlighter, trail.Target.transform.position, Quaternion.identity);
                        
                        _highlighters.Add(effect);
                    }
                }
            }

            yield return null;

            if (_trails.All(t => t.IsComplete()) && _trails.Count == targetPositions.Length)
            {
                break;
            }
        }
        
        while (animator && animator.IsPlaying())
            yield return 0;

        yield return new WaitForSeconds(waitTime);
        
        subEffect = transform.Find("OnDeath");
        if (subEffect) {
            subEffect.SetParent(transform.parent);
            subEffect.gameObject.SetActive(true);
        }
        
        if (sound) sound.Play("OnDeath");
        if (animator) animator.Play("OnDeath");

        foreach (var trail in _trails)
        {
            trail.Target.OnUnpress();
        }

        for (var i = 0; i < _highlighters.Count; i++)
        {
            Destroy(_highlighters[i]);
        }

        onReach();
        if (onReachCoroutine != null && onReachCoroutine.Length > 0)
        {
            for (var i = 0; i < onReachCoroutine.Length; i++)
            {
                var routine = onReachCoroutine[i];
                if (routine != null)
                {
                    if (i == onReachCoroutine.Length - 1)
                    {
                        yield return StartCoroutine(routine);
                    }
                    else
                    {
                        StartCoroutine(routine);
                    }
                }
            }
        }

        ISlotContent.gravityLockers.Remove(this);

        yield return StartCoroutine(Death());

        while (animator && animator.IsPlaying())
            yield return 0;

        isComplete = true;
    }

    private void OnDestroy()
    {
        for (var i = 0; i < _trails.Count; i++)
        {
            Destroy(_trails[i].Trail);
        }
        
        for (var i = 0; i < _highlighters.Count; i++)
        {
            Destroy(_highlighters[i]);
        }
    }

    public IEnumerator GetSoundNames() {
        yield return "OnAwake";
        yield return "OnDeath";
    }

    public IEnumerator GetAnimationNames() {
        yield return "OnAwake";
        yield return "OnDeath";
    }

    public virtual IEnumerator Death() {
        yield return new WaitForSeconds(time);
    }
    
    public Vector3 GetNewPosition(Vector3 startPosition, Vector3 targetPosition, float time)
    {
        return Vector3.Lerp(startPosition, targetPosition + offset, time);
    }
}
