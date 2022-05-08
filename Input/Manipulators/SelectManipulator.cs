using System;
using System.Collections;
using UnityEngine;

namespace Common.Inputs
{
	public class SelectManipulator : Manipulator
	{
		private Action _onClickStart;
		private Action _onClickEnd;

		public SelectManipulator(Manipulatable manipulatable, KeyCode key, Func<KeyCode, bool> inputChecker, Action onClickStart = null, Action onClickEnd = null) : base(manipulatable, key, inputChecker)
		{
			this._onClickStart = onClickStart;
			this._onClickEnd = onClickEnd;
		}

		public override void Start(GameObject[] objs)
		{
			_manipulatable.StartCoroutine(Run());
			_onClickStart?.Invoke();
		}

		private IEnumerator Run()
		{
			while (!Input.GetMouseButtonUp(0))
			{
				yield return null;
			}

			End();
		}

		public override void End()
		{
			this._onClickEnd?.Invoke();
		}
	}
}
