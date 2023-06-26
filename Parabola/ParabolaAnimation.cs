using System;
using Extensions;
using UnityEngine;

public class ParabolaAnimation : MonoBehaviour
{
    [Header("General")]
    public Parabola Parabola;
    public Transform Transform;

    [Header("Animation")]
    public AnimationCurve TimeCurve;
    [Range(0, 1f)] public float TimeAnimation;
    public bool Bezier;

    [Header("Playing")]
    public float Speed;
    public bool PlayOnAwake;
    public bool Reverse;
    public bool Loop;
    public bool PingPong;

    [Header("Events")]
    public PointEvent[] Events = new PointEvent[0];

    public bool IsPlaying { get; private set; }
    
    private bool _reverseTimer;

    public void Pause() => IsPlaying = false;
    public void Continue() => IsPlaying = true;
    public void StartAnimation(bool playOnAwake = true)
    {
        IsPlaying = true;
        PlayOnAwake = playOnAwake;
        if(Reverse)
        {
            TimeAnimation = 1;
            _reverseTimer = true;
        }
        else 
        {
            TimeAnimation = 0;
            _reverseTimer = false;
        }
    }

    protected virtual void Interpolate(float time) 
    {
        if(Transform == null) return;
        var evaluateTime = time * TimeCurve.Evaluate(time);
        var position = Bezier ? Parabola.EvaluateBezier(evaluateTime) : Parabola.Evaluate(evaluateTime);
        Transform.position = position;
#if UNITY_EDITOR
        if(!Application.isPlaying) return;
        foreach (var pointEvent in Events) pointEvent.Enter(time);
#else
        foreach (var pointEvent in Events) pointEvent.Enter(time);
#endif
        
    }

    private void UpdateAnimation()
    {
        if(!IsPlaying) return;
        var direction = Reverse ? -Speed : Speed;
        if(Loop)
        {
            var reverse = PingPong ? _reverseTimer : Reverse;
            direction = reverse ? -Speed : Speed;
            if(TimeAnimation >= 1)
            {
                _reverseTimer = true;
                TimeAnimation = PingPong ? 1 : 0;
            }
            else if(TimeAnimation <= 0)
            {
                _reverseTimer = false;
                TimeAnimation = PingPong ? 0 : 1;
            }
            TimeAnimation += direction * Time.deltaTime;
            Interpolate(TimeAnimation);
            return;
        }
        else TimeAnimation += direction * Time.deltaTime;
        if(TimeAnimation >= 1 || TimeAnimation <= 0)
        {
            TimeAnimation = Reverse ? 0 : 1;
            IsPlaying = false;
        }
        Interpolate(TimeAnimation);
    }
    private void Awake()
    {
        if(PlayOnAwake == false) return;
        StartAnimation();
    }
    private void Update()
    {
        if(PlayOnAwake == false) return;
        UpdateAnimation();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(Application.isPlaying
            || !transform.IsThisOrChild()
            || !Parabola
            || !Transform) return;
        Interpolate(TimeAnimation);
    }
#endif
}