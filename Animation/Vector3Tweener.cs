using UnityEngine;
using System.Collections;

namespace Common.Animation {
	public abstract class Vector3Tweener : Tweener
	{
		public Vector3 StartTweenValue;
		public Vector3 EndTweenValue;
		public Vector3 CurrentTweenValue { get; private set; }

		protected override void OnUpdate ()
		{
			CurrentTweenValue = (EndTweenValue - StartTweenValue) * currentValue + StartTweenValue;
			base.OnUpdate ();
		}
	}
}