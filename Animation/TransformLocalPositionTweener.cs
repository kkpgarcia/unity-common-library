﻿using UnityEngine;
using System.Collections;

namespace Common.Animation {
	public class TransformLocalPositionTweener : Vector3Tweener 
	{
		protected override void OnUpdate ()
		{
			base.OnUpdate ();
			transform.localPosition = CurrentTweenValue;
		}
	}
}