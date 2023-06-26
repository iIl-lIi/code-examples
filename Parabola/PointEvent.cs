using System;
using UnityEngine;
using UnityEngine.Events;

public enum EventEnterType { Ignore, LessOrEqual, BiggerOrEqual }

[System.Serializable] public class PointEvent
{
    public event Action Entered;

    [field: SerializeField] public EventEnterType EnterType { get; private set; }
    [field: SerializeField, Range(0, 1)] public float Point { get; private set; }
    public UnityEvent OnEvent;

    private bool _eventInvoked;

    public void Enter(float value)
    {
        if(EnterType == EventEnterType.Ignore) return;
        
        if(value < 0) value = 0;
        else if(value > 1) value = 1;

        if(EnterType == EventEnterType.LessOrEqual)
        {
            if(!_eventInvoked && value <= Point)
            {
                Entered?.Invoke();
                OnEvent.Invoke();
                _eventInvoked = true;
            }
            else if(value > Point) _eventInvoked = false;
        }
        else if(EnterType == EventEnterType.BiggerOrEqual)
        {
            if(!_eventInvoked && value >= Point)
            {
                OnEvent.Invoke();
                Entered?.Invoke();
                _eventInvoked = true;
            }
            else if(value < Point) _eventInvoked = false;
        }   
    }
}