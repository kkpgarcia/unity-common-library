using System;
using System.Collections;
using UnityEngine;
using Common.Utility;

namespace Common.Inputs
{
	public class HoverManipulator : Manipulator
	{
		protected Action _onHoverEnter;
		protected Action _onHover;
		protected Action _onHoverExit;

		private float _delay;
		private bool _hovering;

		public HoverManipulator(Manipulatable manipulatable, float delay, Action onHoverEnter = null, Action onHover = null, Action onHoverExit = null) : base(manipulatable, KeyCode.None,
			(x) => true)
		{
			_onHoverEnter = onHoverEnter;
			_onHover = onHover;
			_onHoverExit = onHoverExit;

			this._delay = delay;
		}

		public override void Start(GameObject[] objs)
		{
			if (_hovering) return;
			;
			_manipulatable.StartCoroutine(Run());
		}

		protected virtual IEnumerator Run()
		{
			_hovering = true;

			float timer = 0.0f;

			while (timer < this._delay)
			{
				timer += Time.deltaTime;

				if (!this._manipulatable.Transform.IsMouseOver())
				{
					End();
					yield break;
				}
				yield return null;
			}

			this._onHoverEnter?.Invoke();

			while (this._manipulatable.Transform.IsMouseOver())
			{
				this._onHover?.Invoke();
				yield return null;
			}

			this._onHoverExit?.Invoke();
			End();
		}

		public override void End()
		{
			_hovering = false;
			_manipulatable.StopCoroutine(Run());
		}
	}
}
