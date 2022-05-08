using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Common.UI
{
    public class UIPointerEvents : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
	    [HideInInspector]
        public UnityEvent OnPointerUpEvent = null;
        [HideInInspector]
        public UnityEvent OnPointerDownEvent = null;
        [HideInInspector]
        public UnityEvent OnPointerEnterEvent = null;
        [HideInInspector]
        public UnityEvent OnPointerExitEvent = null;

        [SerializeField] private PointerEventData.InputButton _requiredInput;

        protected Camera _camera;

        public void Setup(Camera camera)
        {
	        this._camera = camera;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
	        if (eventData.button != this._requiredInput) return;

            OnPointerUpEvent?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
	        if (eventData.button != this._requiredInput) return;

            OnPointerDownEvent?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
	        if (eventData.button != this._requiredInput) return;

	        OnPointerEnterEvent?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
	        if (eventData.button != this._requiredInput) return;

            OnPointerExitEvent?.Invoke();
        }

        protected Vector2 GetPositionRelativeToCamera(Vector2 position)
        {
	        if (_camera == null)
	        {
		        _camera = Camera.main;
	        }
	        return _camera.ScreenToWorldPoint(position);
        }
    }
}
