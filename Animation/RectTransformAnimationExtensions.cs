﻿using UnityEngine;
using System;
using System.Collections;

namespace Common.Animation {
	public static class RectTransformAnimationExtensions 
	{
		public static Tweener AnchorTo (this RectTransform t, Vector3 position)
		{
			return AnchorTo (t, position, Tweener.DefaultDuration);
		}
		
		public static Tweener AnchorTo (this RectTransform t, Vector3 position, float duration)
		{
			return AnchorTo (t, position, duration, Tweener.DefaultEquation);
		}
		
		public static Tweener AnchorTo (this RectTransform t, Vector3 position, float duration, Func<float, float, float, float> equation)
		{
			RectTransformAnchorPositionTweener tweener = t.gameObject.AddComponent<RectTransformAnchorPositionTweener> ();
			tweener.StartTweenValue = t.anchoredPosition;
			tweener.EndTweenValue = position;
			tweener.Property.duration = duration;
			tweener.Property.equation = equation;
			tweener.Play ();
			return tweener;
		}
	}
}