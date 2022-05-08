using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Common.UI
{
	public class UIMover : UIPointerEvents
	{
		[SerializeField] private RectTransform _parent;
		private bool _isLocked = false;

		private Vector2 _mouseOffset;
		private bool _isDragged = false;

		[SerializeField]
		private bool _snapToGrid = false;
		[SerializeField]
		private int _snapValue = 2;

		public UnityEvent OnMoveStartEvent;
		public UnityEvent OnMovingEvent;
		public UnityEvent OnMoveEndEvent;


		void Start()
		{
			this.OnPointerDownEvent.AddListener(SetIsDragged);
		}

		void Update()
		{
			if (!_isDragged || _isLocked) return;

			Vector3 newPos = this._parent.position;

			if (_snapToGrid)
			{
				int dx = Mathf.FloorToInt(GetPositionRelativeToCamera(Input.mousePosition).x - newPos.x);
				int dy = Mathf.FloorToInt(GetPositionRelativeToCamera(Input.mousePosition).y - newPos.y);

				newPos.x = dx > _snapValue ? newPos.x + _snapValue : dx < -_snapValue ? newPos.x - _snapValue : newPos.x;
				newPos.y = dy > _snapValue ? newPos.y + _snapValue : dy < -_snapValue ? newPos.y - _snapValue : newPos.y;
			}
			else
			{
				Vector2 camPos = GetPositionRelativeToCamera(Input.mousePosition) + _mouseOffset;
				newPos = new Vector3(camPos.x, camPos.y, 0);
			}

			_parent.position = new Vector3(newPos.x, newPos.y, 0);

			_parent.localPosition = new Vector3(_parent.localPosition.x, _parent.localPosition.y, 0);
			OnMovingEvent?.Invoke();

			if (Input.GetMouseButtonUp(0))
			{
				_isDragged = false;
				OnMoveEndEvent?.Invoke();
			}
		}

		public void SetIsDragged()
		{
			if (_isLocked) return;

			Vector2 camPos = GetPositionRelativeToCamera(Input.mousePosition);
			_mouseOffset = _parent.position - new Vector3(camPos.x, camPos.y, 0);// ((Vector3)GetPositionRelativeToCamera(Input.mousePosition));
			_isDragged = true;
			_parent.SetAsLastSibling();
			OnMoveStartEvent?.Invoke();
		}

		public void SetLocked(bool value)
		{
			this._isLocked = value;
		}
	}
}
