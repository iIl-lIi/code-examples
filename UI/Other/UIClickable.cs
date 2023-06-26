using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Game.UI.Other
{ 
    public class UIClickable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public event Action<PointerEventData> Enter;
        public event Action<PointerEventData> Exit;
        public event Action<PointerEventData> Down;
        public event Action<PointerEventData> Up;
        
        [SerializeField] private UnityEvent _EnterEvent; 
        [SerializeField] private UnityEvent _ExitEvent;
        [SerializeField] private UnityEvent _DownEvent; 
        [SerializeField] private UnityEvent _UpEvent; 
    
        public void OnPointerEnter(PointerEventData eventData)
        {
            Enter?.Invoke(eventData);
            
            _EnterEvent.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Exit?.Invoke(eventData);
            
            _ExitEvent.Invoke();
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            Down?.Invoke(eventData);
            
            _DownEvent.Invoke();
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            Up?.Invoke(eventData);
            
            _UpEvent.Invoke();
        }
    }
}