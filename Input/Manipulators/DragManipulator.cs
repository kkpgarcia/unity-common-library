using System;
using System.Collections;
using UnityEngine;

namespace Common.Inputs
{
	public class DragManipulator : Manipulator
	{
		private Action<Vector2> _onDragStart;
		private Action<Vector2> _onDrag;
		private Action<Vector2, ISlot> _onDragEnd;

		private float _delay;
		private bool _isDragging;

		public DragManipulator(Manipulatable manipulatable, KeyCode key, Func<KeyCode, bool> inputChecker, float delay, Action<Vector2> onDragStart = null, Action<Vector2> onDrag = null, Action<Vector2, ISlot> onDragEnd = null) : base (manipulatable, key, inputChecker)
		{
			this._delay = delay;

			_onDragStart = onDragStart;
			_onDrag = onDrag;
			_onDragEnd = onDragEnd;
		}

		public override void Start(GameObject[] objs)
		{
			if (this._isDragging) return;

			_manipulatable.StartCoroutine(Run());
		}

		protected virtual IEnumerator Run()
		{
			_isDragging = true;

			float timer = 0.0f;

			while (timer < this._delay)
			{
				timer += Time.deltaTime;
				if (Input.GetMouseButtonUp(0))
				{
					End();
					yield break;
				}
				yield return null;
			}

			_onDragStart?.Invoke(Input.mousePosition);

			ISlot slot = null;

			while (!Input.GetMouseButtonUp(0))
			{
				Vector2 mousePosition = Input.mousePosition;
				// GameObject slottedObject = _inputController.GetCurrentObjects().FirstOrDefault(x => x.GetComponent<Slot>());//.GetComponent<Slot>();

				// if (slottedObject != null)
				// {
				// 	slot = slottedObject.GetComponent<Slot>();
				// 	Debug.Log(slot);
				// }

				this.OnDropHover(slot);
				_onDrag?.Invoke(mousePosition);
				yield return null;
			}

			this.OnDropHandler(slot);
			_onDragEnd?.Invoke(Input.mousePosition, slot);

			End();
		}

		protected void OnDropHover(ISlot slot)
		{
			if (slot == null) return;

			slot.OnDragHover(this._manipulatable);
		}

		protected void OnDropHandler(ISlot slot)
		{
			if (slot == null) return;

			slot.OnReceive(this._manipulatable);
		}

		public override void End()
		{
			_manipulatable.StopCoroutine(Run());
			_isDragging = false;
		}
	}
}
