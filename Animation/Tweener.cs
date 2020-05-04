using UnityEngine;
using System;
using System.Collections;

namespace Common.Animation {
	public abstract class Tweener : EasingControl
	{
		#region Properties
		public static float DefaultDuration = 1f;
		public static Func<float, float, float, float> DefaultEquation = EasingEquations.EaseInOutQuad;
		public bool DestroyOnComplete = true;
		#endregion

		#region Event Handlers
		protected override void OnComplete ()
		{
			base.OnComplete ();
			if (DestroyOnComplete)
				Destroy(this);
		}
		#endregion
	}
}