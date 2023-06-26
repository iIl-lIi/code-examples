using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Game.UI.Other
{
    public class UIDragable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public event Action<PointerEventData> BeginDrag;
        public event Action<PointerEventData> Drag;
        public event Action<PointerEventData> EndDrag;

        [SerializeField] private UnityEvent _BeginEvent;
        [SerializeField] private UnityEvent _EndEvent;
    
        public void OnBeginDrag(PointerEventData eventData)
        {
            BeginDrag?.Invoke(eventData);
            _BeginEvent.Invoke();
        }
    
        public void OnDrag(PointerEventData eventData)
        {
            Drag?.Invoke(eventData);
        }
    
        public void OnEndDrag(PointerEventData eventData)
        {
            EndDrag?.Invoke(eventData);
            _EndEvent.Invoke();
        }
    }
}