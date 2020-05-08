using UnityEngine;
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
			return AnchorTo(t, new RectTransformAnimationProperty() {
				Offset = position,
				duration = duration,
				equation = equation,
			});
		}

		public static Tweener AnchorTo(this RectTransform t, RectTransformAnimationProperty property) {
			RectTransformAnchorPositionTweener tweener = t.gameObject.AddComponent<RectTransformAnchorPositionTweener> ();

			//Temporary Boxing and Unboxing of reference types
			
			//Vector2 offset = (Vector2)boxedOffset;

			tweener.Property = property;
			tweener.StartTweenValue = t.anchoredPosition;
			tweener.EndTweenValue = property.Offset;
			tweener.Play ();
			return tweener;
		}
	}
}