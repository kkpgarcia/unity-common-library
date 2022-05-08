using UnityEngine;
using UnityEngine.Events;

namespace Common.UI
{
    public class UIHandle : UIPointerEvents
    {
        [SerializeField] private RectTransform _parent = null;
        [SerializeField] private Texture2D _cursor = null;
        [SerializeField] private Axis _axis = Axis.Horizontal;
        private bool _isLocked = false;
        [SerializeField] private float _minWidth = 100;
        [SerializeField] private float _minHeight = 100;
        [SerializeField] private float _maxWidth = 1000;
        [SerializeField] private float _naxHeight = 1000;
        private Direction _direction;
        private bool _isDragged = false;
        private Vector2 _initMousePos;
        private Vector2 _initSize;
        private Vector2 _initPivot;

        public UnityEvent OnDragEvent;
        public UnityEvent OnDragStartEvent;
        public UnityEvent OnDragEndEvent;


        private void Start()
        {
	        OnPointerDownEvent.AddListener(SetIsDragged);
            OnPointerDownEvent.AddListener(_parent.SetAsLastSibling);
            OnPointerEnterEvent.AddListener(ShowCursor);
            OnPointerExitEvent.AddListener(ResetCursor);

            switch (_axis)
            {

                case Axis.Horizontal:
                    if (transform.position.x > _parent.position.x)
                    {
                        _direction = Direction.Right;
                    }
                    else
                    {
                        _direction = Direction.Left;
                    }
                    break;

                case Axis.Vertical:
                    if (transform.position.y > _parent.position.y)
                    {
                        _direction = Direction.Up;
                    }
                    else
                    {
                        _direction = Direction.Down;
                    }
                    break;

                case Axis.Diagonal:
                    if (transform.position.y > _parent.position.y)
                    {
                        if (transform.position.x > _parent.position.x)
                        {
                            _direction = Direction.UpRight;
                        }
                        else
                        {
                            _direction = Direction.UpLeft;
                        }
                    }
                    else
                    {
                        if (transform.position.x > _parent.position.x)
                        {
                            _direction = Direction.DownRight;
                        }
                        else
                        {
                            _direction = Direction.DownLeft;
                        }
                    }
                    break;
            }
        }

        void Update()
        {
            if (!_isDragged) return;

            if (Input.GetMouseButtonUp(0))
            {
                _isDragged = false;
                this._parent.SetPivot(_initPivot);
                OnDragEndEvent?.Invoke();
                return;
            }

            Vector2 offset = (Vector2.one - (Vector2)transform.lossyScale) + Vector2.one;
            Vector2 delta = Vector2.Scale((Vector2)Input.mousePosition - _initMousePos, offset);
            Vector2 size = _initSize;

            switch (_direction)
            {
                case Direction.Up:
                    size += new Vector2(0, delta.y);
                    break;
                case Direction.Down:
                    size -= new Vector2(0, delta.y);
                    break;
                case Direction.Left:
                    size -= new Vector2(delta.x, 0);
                    break;
                case Direction.Right:
                    size += new Vector2(delta.x, 0);
                    break;
                case Direction.UpRight:
					size += new Vector2(delta.x, delta.y);
					break;
				case Direction.UpLeft:
					size += new Vector2(-delta.x, delta.y);
					break;
				case Direction.DownRight:
					size += new Vector2(delta.x, -delta.y);
					break;
				case Direction.DownLeft:
					size += new Vector2(-delta.x, -delta.y);
					break;
            }

            if (size.x < _minWidth || size.y < _minHeight || size.x > _maxWidth || size.y > _naxHeight)
            {
                Vector2 newSize = size;

                if (size.x < _minWidth)
                    newSize.x = _minWidth;

                if (size.y < _minHeight)
                    newSize.y = _minHeight;

                if (size.x > _maxWidth)
	                newSize.x = _maxWidth;

                if (size.y > _naxHeight)
	                newSize.y = _naxHeight;

                _parent.sizeDelta = newSize;
                return;
            }

            _parent.sizeDelta = size;

            OnDragEvent?.Invoke();
        }

        public void SetLocked(bool input)
        {
            _isLocked = input;
            _parent.SetAsLastSibling();
        }

        public void SetIsDragged()
        {
            if (_isLocked) return;

            _isDragged = true;

            OnDragStartEvent?.Invoke();

            _initMousePos = Input.mousePosition;
            _initSize = _parent.sizeDelta;
            _initPivot = _parent.pivot;

            switch (_direction)
            {
                case Direction.Up:
					_parent.SetPivot(new Vector2(0.5f, 0));
					break;
				case Direction.Down:
					_parent.SetPivot(new Vector2(0.5f, 1));
					break;
				case Direction.Left:
					_parent.SetPivot(new Vector2(1, 0.5f));
					break;
				case Direction.Right:
					_parent.SetPivot(new Vector2(0, 0.5f));
					break;
				case Direction.UpRight:
					_parent.SetPivot(new Vector2(0, 0));
					break;
				case Direction.UpLeft:
					_parent.SetPivot(new Vector2(1, 0));
					break;
				case Direction.DownRight:
					_parent.SetPivot(new Vector2(0, 1));
					break;
				case Direction.DownLeft:
					_parent.SetPivot(new Vector2(1, 1));
					break;
            }

            _parent.SetAsLastSibling();
        }

        public void ShowCursor()
        {
            if (!_isLocked && _cursor != null)
            {
                Cursor.SetCursor(_cursor, new Vector2(16, 16), CursorMode.Auto);
            }
        }

        public void ResetCursor()
        {
            if (_cursor != null)
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
        }
    }
}
