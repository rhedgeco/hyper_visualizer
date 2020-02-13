using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HyperScripts
{
    [System.Serializable]
    public class TimelineSlider : Slider
    {
        public UnityEvent OnRetargetSlider { get; } = new UnityEvent();
        
        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
            OnRetargetSlider.Invoke();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            OnRetargetSlider.Invoke();
        }
    }
}